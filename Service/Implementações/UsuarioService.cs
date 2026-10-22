using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.Usuario;
using Controle_De_estoqueAPI.Models;
using Microsoft.EntityFrameworkCore;
using Controle_De_estoqueAPI.Service.Interfaces;



namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ControleEstoqueDbContexto _context;

        private readonly IGerarHashService _gerarHash;
        private readonly IPerfilService _perfil;

        public UsuarioService(ControleEstoqueDbContexto context, 
            IGerarHashService gerarHash,
            IPerfilService perfilService)
        {
            _context = context;
            _gerarHash = gerarHash;
            _perfil = perfilService;

        }


        // Métodos públicos da interface
        public async Task<UsuarioRespostaDto> CriarAsync(CriarUsuarioDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
                throw new ArgumentException("Email não pode ser vazio");

            var periflExiste = await _perfil.ObterPorIdAsync(dto.PerfilId);

            if (periflExiste == null) 
            {
                throw new InvalidOperationException("Perfil Invalido.");
            }

            var existente = await _context.Usuarios.FirstOrDefaultAsync(u => u.EmailUsuario == dto.Email);

            if (existente != null)
            {        // Pode lançar uma exceção customizada ou retornar null
                throw new InvalidOperationException("Já existe um usuário com este e-mail.");
            }
          
            var usuario = 
            new Usuario
            {
                NomeUsuario = dto.Nome,
                EmailUsuario = dto.Email,
                Login = dto.Login,
                SenhaHash = _gerarHash.GerarHash(dto.Senha), // Hash da senha com fator de trabalho 12
                PerfilId = dto.PerfilId,               
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return new UsuarioRespostaDto 
            { Id = usuario.Id,
                Nome = usuario.NomeUsuario,
                Email = usuario.EmailUsuario,
                Login = dto.Login,
                PerfilId = dto.PerfilId,
                
            };
        }

        public async Task<IEnumerable<UsuarioRespostaBuscaDto>> ListarAsync()
        {
            var usuarios = await _context.Usuarios.Select(u => new UsuarioRespostaBuscaDto
            {
                Id = u.Id,
                Nome = u.NomeUsuario,
                Email = u.EmailUsuario,
                PerfilId = u.PerfilId,
                UltimoLogin = u.UltimoLogin,
                Login = u.Login,
                Ativo = u.Ativo,
                DataCriacao = u.DataCriacao
            })

              .ToListAsync();
            return usuarios;
        }

        public async Task<UsuarioRespostaDto> AtualizarAsync(int id, AtualizarUsuarioDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id)
            ?? throw new KeyNotFoundException($"Usuário {id} não encontrado.");


            usuario.NomeUsuario = dto.Nome;
            usuario.EmailUsuario = dto.Email;
            usuario.PerfilId = dto.PerfilId;


            await _context.SaveChangesAsync();

            return new UsuarioRespostaDto
            { Nome = usuario.NomeUsuario,
                Email = usuario.EmailUsuario,
                PerfilId = usuario.PerfilId,
                Login = usuario.Login,
            };
        }

        public async Task<UsuarioRespostaBuscaDto> ObterPorIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id inválido");
            }

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                throw new KeyNotFoundException($"Usuário {id} não encontrado.");
            }

            return new UsuarioRespostaBuscaDto
            {
                Id = usuario.Id,
                Nome = usuario.NomeUsuario,
                Email = usuario.EmailUsuario,
                PerfilId = usuario.PerfilId,
                UltimoLogin = usuario.UltimoLogin,
                DataCriacao = usuario.DataCriacao,
                Ativo = usuario.Ativo,
                Login = usuario.Login,
                
            };
        }

        public async Task<ExclusaoRespostaDto> ExcluirAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return new ExclusaoRespostaDto{Sucesso = false, Mensagem = "Usuario não encontrado"};
            }
            usuario.Ativo = false; // Marca como inativo em vez de excluir fisicamente
            await _context.SaveChangesAsync();
            return new ExclusaoRespostaDto{Sucesso = true, Mensagem = "Usuario excluído com sucesso"};
        }
    }
}

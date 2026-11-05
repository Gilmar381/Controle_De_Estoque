using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.Usuario;
using Controle_De_estoqueAPI.Exceptions;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        private static UsuarioRespostaDto Mapear(Usuario u) => new()
        {
            Id = u.Id,
            Nome = u.NomeUsuario,
            Email = u.EmailUsuario,
            Login = u.Login,
            PerfilId = u.PerfilId,
            Ativo = u.Ativo
        };

        public async Task<UsuarioRespostaDto> CriarAsync(CriarUsuarioDto dto)
        {
            if (await _perfil.ObterPorIdAsync(dto.PerfilId) == null)
                throw new RegraDeNegocioException("Perfil inválido.");

            if (await _context.Usuarios.AnyAsync(u => u.EmailUsuario == dto.Email))
                throw new ConflitoException("Já existe um usuário com este e-mail.");

            if (await _context.Usuarios.AnyAsync(u => u.Login == dto.Login))
                throw new ConflitoException("Já existe um usuário com este login.");

            var usuario = new Usuario
            {
                NomeUsuario = dto.Nome,
                EmailUsuario = dto.Email,
                Login = dto.Login,
                SenhaHash = _gerarHash.GerarHash(dto.Senha), // BCrypt, fator de trabalho 10
                PerfilId = dto.PerfilId,
                Ativo = dto.Ativo,
                DataCriacao = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Mapear(usuario);
        }

        public async Task<IEnumerable<UsuarioRespostaBuscaDto>> ListarAsync()
        {
            return await _context.Usuarios
                .AsNoTracking()
                .OrderBy(u => u.NomeUsuario)
                .Select(u => new UsuarioRespostaBuscaDto
                {
                    Id = u.Id,
                    Nome = u.NomeUsuario,
                    Email = u.EmailUsuario,
                    Login = u.Login,
                    PerfilId = u.PerfilId,
                    PerfilNome = u.Perfil.PerfilNome,
                    Ativo = u.Ativo,
                    DataCriacao = u.DataCriacao,
                    UltimoLogin = u.UltimoLogin
                })
                .ToListAsync();
        }

        public async Task<UsuarioRespostaBuscaDto> ObterPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id inválido");

            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UsuarioRespostaBuscaDto
                {
                    Id = u.Id,
                    Nome = u.NomeUsuario,
                    Email = u.EmailUsuario,
                    Login = u.Login,
                    PerfilId = u.PerfilId,
                    PerfilNome = u.Perfil.PerfilNome,
                    Ativo = u.Ativo,
                    DataCriacao = u.DataCriacao,
                    UltimoLogin = u.UltimoLogin
                })
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException($"Usuário {id} não encontrado.");
        }

        public async Task<UsuarioRespostaDto> AtualizarAsync(int id, AtualizarUsuarioDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id)
                ?? throw new KeyNotFoundException($"Usuário {id} não encontrado.");

            if (dto.Nome != null)
            {
                if (string.IsNullOrWhiteSpace(dto.Nome))
                    throw new RegraDeNegocioException("O nome não pode ser vazio.");
                usuario.NomeUsuario = dto.Nome;
            }

            if (dto.Email != null && dto.Email != usuario.EmailUsuario)
            {
                if (await _context.Usuarios.AnyAsync(u => u.EmailUsuario == dto.Email && u.Id != id))
                    throw new ConflitoException("Já existe um usuário com este e-mail.");
                usuario.EmailUsuario = dto.Email;
            }

            if (dto.PerfilId.HasValue)
            {
                if (await _perfil.ObterPorIdAsync(dto.PerfilId.Value) == null)
                    throw new RegraDeNegocioException("Perfil inválido.");
                usuario.PerfilId = dto.PerfilId.Value;
            }

            if (dto.Ativo.HasValue)
                usuario.Ativo = dto.Ativo.Value;

            await _context.SaveChangesAsync();
            return Mapear(usuario);
        }

        public async Task<ExclusaoRespostaDto> ExcluirAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id)
                ?? throw new KeyNotFoundException("Usuário não encontrado.");

            usuario.Ativo = false; // exclusão lógica
            await _context.SaveChangesAsync();

            return new ExclusaoRespostaDto { Sucesso = true, Mensagem = "Usuário desativado com sucesso" };
        }
    }
}

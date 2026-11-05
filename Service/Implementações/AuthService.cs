using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.Usuario;
using Controle_De_estoqueAPI.Exceptions;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class AuthService : IAuthService
    {
        private readonly ControleEstoqueDbContexto _context;
        private readonly IGerarHashService _gerarHash;
        private readonly ITokenService _tokenService;

        public AuthService(ControleEstoqueDbContexto context, IGerarHashService hasher, ITokenService token)
        {
            _context = context;
            _gerarHash = hasher;
            _tokenService = token;
        }

        public async Task<LoginRespostaDto> LoginAsync(LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Perfil)
                .FirstOrDefaultAsync(u => u.Login == dto.Login);

            if (usuario != null && usuario.Ativo && _gerarHash.VerificarHash(dto.Senha, usuario.SenhaHash))
            {
                usuario.UltimoLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new LoginRespostaDto
                {
                    Nome = usuario.NomeUsuario,
                    Token = _tokenService.GerarToken(usuario),
                    UsuarioId = usuario.Id,
                    PerfilNome = usuario.Perfil.PerfilNome,
                    Mensagem = "Login bem-sucedido",
                    Sucesso = true
                };
            }

            return new LoginRespostaDto
            {
                Mensagem = "Login ou senha inválidos",
                Sucesso = false
            };
        }

        public async Task<AlterarSenhaRespostaDto> AlterarSenhaAsync(int id, AlterarSenhaDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id)
                ?? throw new KeyNotFoundException($"Usuário {id} não encontrado.");

            if (!_gerarHash.VerificarHash(dto.SenhaAtual, usuario.SenhaHash))
                throw new RegraDeNegocioException("Senha atual incorreta.");

            if (dto.SenhaAtual == dto.NovaSenha)
                throw new RegraDeNegocioException("A nova senha deve ser diferente da atual.");

            usuario.SenhaHash = _gerarHash.GerarHash(dto.NovaSenha);
            await _context.SaveChangesAsync();

            return new AlterarSenhaRespostaDto { Mensagem = "Senha alterada com sucesso." };
        }
    }
}

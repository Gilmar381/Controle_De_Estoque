using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.Usuario;
using Microsoft.EntityFrameworkCore;
using Controle_De_estoqueAPI.Service.Interfaces;

namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class AuthService : IAuthService
    {
        private readonly ControleEstoqueDbContexto _context;

        private readonly IGerarHashService _gerarHashe;
        private readonly ITokenService _tokenService;

        public AuthService(ControleEstoqueDbContexto context, IGerarHashService Hasher, ITokenService token)
        {
            _context = context;
            _gerarHashe =  Hasher;
            _tokenService = token;

        }
        public async Task<LoginRespostaDto> LoginAsync(LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Perfil)
                .FirstOrDefaultAsync(u => u.Login == dto.Login);


            if (usuario != null && usuario.Ativo && _gerarHashe.VerificarHash(dto.Senha, usuario.SenhaHash))
            {
                usuario.UltimoLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return new LoginRespostaDto 
                { Nome = usuario.NomeUsuario,
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
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                throw new KeyNotFoundException($"Usuário {id} não encontrado.");
            }
            if (!_gerarHashe.VerificarHash(dto.SenhaAtual, usuario.SenhaHash))
            {
                throw new UnauthorizedAccessException("Senha atual incorreta.");
            }
            usuario.SenhaHash = _gerarHashe.GerarHash(dto.NovaSenha);
            await _context.SaveChangesAsync();
            return new AlterarSenhaRespostaDto { Mensagem = "Senha alterada com sucesso." };
        }
    }
}

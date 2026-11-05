using System.Security.Claims;

namespace Controle_De_estoqueAPI.Extensions
{
    public static class UsuarioLogadoExtensions
    {
        /// <summary>Lê o ID do usuário autenticado a partir do token JWT.</summary>
        public static int ObterUsuarioId(this ClaimsPrincipal user)
        {
            var valor = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(valor, out var id)
                ? id
                : throw new UnauthorizedAccessException("Usuário não identificado no token.");
        }
    }
}

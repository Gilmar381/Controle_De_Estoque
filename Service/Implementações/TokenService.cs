using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Interfaces;
namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuracao;

        public TokenService(IConfiguration config)
        {
            _configuracao = config;
        }

        public string GerarToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NomeUsuario),
                new Claim(ClaimTypes.Role, usuario.Perfil.PerfilNome)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracao["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuracao["Jwt:Issuer"],
                audience: _configuracao["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

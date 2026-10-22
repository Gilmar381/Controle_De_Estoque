using Controle_De_estoqueAPI.Dtos.Usuario;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Controle_De_estoqueAPI.Constantes;

namespace Controle_De_estoqueAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

       public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var resultado = await _authService.LoginAsync(dto);
            if (resultado.Token == null) // ou alguma flag de sucesso
                return Unauthorized(resultado); // 401
            return Ok(resultado);
        }

        [HttpPut("Login")]
        [Authorize]
        public async Task<IActionResult> AlterarSenha( int id, [FromBody] AlterarSenhaDto dto)
        {
            var resultado = await _authService.AlterarSenhaAsync(id ,dto);
            return Ok(resultado);
        }


    }
}

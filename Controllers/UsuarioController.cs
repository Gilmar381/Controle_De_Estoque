using Microsoft.AspNetCore.Mvc;
using Controle_De_estoqueAPI.Service.Interfaces;
using Controle_De_estoqueAPI.Dtos.Usuario;
using Controle_De_estoqueAPI.Constantes;
using Microsoft.AspNetCore.Authorization;

namespace Controle_De_estoqueAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = PerfilConst.Gerente)]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        

        public UsuarioController(IUsuarioService usuarioService, IPerfilService perfilService)
        {
            _usuarioService = usuarioService;
            
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodos()
        {
            var resultado = await _usuarioService.ListarAsync();
            return Ok (resultado);
        }


        [HttpGet("{Id}")]
        public async Task<IActionResult> BuscarPorID(int Id)
        {
            var resultado = await _usuarioService.ObterPorIdAsync(Id);
            return Ok (resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarUsuarioDto dto)
        {
            var resposta = await _usuarioService.CriarAsync(dto);
            return Ok (resposta);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Atualizar( int id, AtualizarUsuarioDto dto)
        {
            var resultado = await _usuarioService.AtualizarAsync(id, dto);
            return Ok(resultado);
        }
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Excluir (int id)
        {
            var resultado = await _usuarioService.ExcluirAsync(id);
            return Ok(resultado);
        }





    }
}

using Controle_De_estoqueAPI.Dtos;
using Controle_De_estoqueAPI.Dtos.Categoria;
using Controle_De_estoqueAPI.Service.Implementações;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Controle_De_estoqueAPI.Constantes;

namespace Controle_De_estoqueAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = PerfilConst.Estoquista + "," + PerfilConst.Gerente)]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodos()
        {
            var resultado = await _categoriaService.ListarAsync();
            return Ok(resultado);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> BuscarPorId(int Id)
        {
            var resultado = await _categoriaService.ObterPorIdAsync(Id);
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarCategoriaDto dto)
        {
            var resultado = await _categoriaService.CriarAsync(dto);
            return Ok(resultado);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Atualizar(int Id, [FromBody] AtualizarCategoriaDto dto)
        {
            var resultado = await _categoriaService.AtualizarAsync(Id, dto);
            return Ok(resultado);
        }


        [HttpDelete("{Id}")]
        public async Task<IActionResult> Excluir(int Id)
        {
            try
            {
                await _categoriaService.ExcluirAsync(Id);
                return NoContent(); // 204 - sucesso, sem corpo de resposta
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // 404
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // 400
            }
        }

    }
}

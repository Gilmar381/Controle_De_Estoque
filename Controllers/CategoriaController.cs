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
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var resultado = await _categoriaService.ObterPorIdAsync(id);
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarCategoriaDto dto)
        {
            var resultado = await _categoriaService.CriarAsync(dto);
            return Ok(resultado);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarCategoriaDto dto)
        {
            var resultado = await _categoriaService.AtualizarAsync(id, dto);
            return Ok(resultado);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                await _categoriaService.ExcluirAsync(id);
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

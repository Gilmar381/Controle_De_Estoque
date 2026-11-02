using Controle_De_estoqueAPI.Constantes;
using Controle_De_estoqueAPI.Dtos.Fornecedor;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controle_De_estoqueAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = PerfilConst.Gerente)]
    public class FornecedorController : ControllerBase
    {
        private readonly IFornecedorService _fornecedorService;

       public FornecedorController(IFornecedorService fornecedorService) 
        {   
            _fornecedorService = fornecedorService;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodos()
        {
            var resultado = await _fornecedorService.ListarAsync();
            return Ok(resultado);
        }

        [HttpGet("{Id}")]

         public async Task<IActionResult> BuscarPorId(int Id)
        {
            var resultado = await _fornecedorService.ObterPorIdAsync(Id);
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarFornecedorDto dto)
        {
            var resultado = await _fornecedorService.CriarAsync(dto);
            return Ok(resultado);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Atualizar(int Id, [FromBody] AtualizarFornecedorDto dto)
        {
            var resultado = await _fornecedorService.AtualizarAsync(Id, dto);
            return Ok(resultado);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Excluir(int Id)
        {
            try
            {
                await _fornecedorService.ExcluirAsync(Id);
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

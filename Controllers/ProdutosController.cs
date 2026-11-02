using Microsoft.AspNetCore.Mvc;
using Controle_De_estoqueAPI.Service.Interfaces;
using Controle_De_estoqueAPI.Dtos.Produto;
using Microsoft.AspNetCore.Authorization;
using Controle_De_estoqueAPI.Constantes;

namespace Controle_De_estoqueAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = PerfilConst.Gerente + "," + PerfilConst.Estoquista)]

    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodos()
        {
            var resultado = await _produtoService.ListarAsync();
            return Ok(resultado);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> BuscarPorId(int Id)
        {
            var resultado = await _produtoService.ObterPorIdAsync(Id);
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(CriarProdutoDto dto)
        {
            var resultado = await _produtoService.CriarAsync(dto);
            return Ok(resultado);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Atualizar(int Id, AtualizarProdutoDto dto)
        {
            var resultado = await _produtoService.AtualizarAsync(Id, dto);
            return Ok(resultado);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> Excluir(int Id)
        {
            var resultado = await _produtoService.ExcluirAsync(Id);
            return Ok(resultado);
        }
    }
}

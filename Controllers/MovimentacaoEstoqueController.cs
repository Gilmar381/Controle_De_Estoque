using Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Controle_De_estoqueAPI.Constantes;

namespace Controle_De_estoqueAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = PerfilConst.Gerente + "," + PerfilConst.Estoquista)]
    public class MovimentacaoEstoqueController : ControllerBase
    {
        private readonly IMovimentacaoService _movimentacaoService;

        public MovimentacaoEstoqueController(IMovimentacaoService movimentacaoService)
        {
            _movimentacaoService = movimentacaoService;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodasMovimentacoes()
        {
            var resposta = await _movimentacaoService.ListarAsync();
            return Ok(resposta);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var resposta = await _movimentacaoService.ObterPorIdAsync(id);
            return Ok(resposta);
        }

        [HttpGet("movimentação/produto/{ProdutoId}")]
        public async Task<IActionResult> BuscarPorProduto(int ProdutoId)
        {
            var resposta = await _movimentacaoService.ListarPorProdutoAsync(ProdutoId);
            return Ok(resposta);
        }

        [HttpGet("movimentação/periodo")]
        public async Task<IActionResult> BuscarPorPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            var resposta = await _movimentacaoService.ListarPorPeriodoAsync(dataInicio, dataFim);
            return Ok(resposta);
        }

        [HttpPost("entrada")]
        public async Task<IActionResult> RegistrarEntrada([FromBody] CriarMovimentacaoDto dto)
        {
            var resposta = await _movimentacaoService.RegistrarEntradaAsync(dto);
            return Ok(resposta);
        }

        [HttpPost("saida")]
        public async Task<IActionResult> RegistrarSaida([FromBody] RegistrarSaidaDto dto)
        {
            var resposta = await _movimentacaoService.RegistrarSaidaAsync(dto);
                return Ok(resposta);
        }
    }
}

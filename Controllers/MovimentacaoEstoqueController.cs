using Controle_De_estoqueAPI.Constantes;
using Controle_De_estoqueAPI.Dtos.Comum;
using Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque;
using Controle_De_estoqueAPI.Extensions;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> BuscarTodasMovimentacoes([FromQuery] PaginacaoParametros paginacao)
        {
            var resposta = await _movimentacaoService.ListarAsync(paginacao);
            return Ok(resposta);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var resposta = await _movimentacaoService.ObterPorIdAsync(id);
            return Ok(resposta);
        }

        [HttpGet("movimentacao/produto/{produtoId:int}")]
        public async Task<IActionResult> BuscarPorProduto(int produtoId, [FromQuery] PaginacaoParametros paginacao)
        {
            var resposta = await _movimentacaoService.ListarPorProdutoAsync(produtoId, paginacao);
            return Ok(resposta);
        }

        [HttpGet("movimentacao/periodo")]
        public async Task<IActionResult> BuscarPorPeriodo(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] PaginacaoParametros paginacao)
        {
            var resposta = await _movimentacaoService.ListarPorPeriodoAsync(dataInicio, dataFim, paginacao);
            return Ok(resposta);
        }

        [HttpPost("entrada")]
        public async Task<IActionResult> RegistrarEntrada([FromBody] CriarMovimentacaoDto dto)
        {
            var resposta = await _movimentacaoService.RegistrarEntradaAsync(dto, User.ObterUsuarioId());
            return Ok(resposta);
        }

        [HttpPost("saida")]
        public async Task<IActionResult> RegistrarSaida([FromBody] RegistrarSaidaDto dto)
        {
            var resposta = await _movimentacaoService.RegistrarSaidaAsync(dto, User.ObterUsuarioId());
            return Ok(resposta);
        }
    }
}
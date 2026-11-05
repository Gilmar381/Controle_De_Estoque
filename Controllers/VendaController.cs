using Controle_De_estoqueAPI.Constantes;
using Controle_De_estoqueAPI.Dtos.Venda;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Controle_De_estoqueAPI.Dtos.Comum;


namespace Controle_De_estoqueAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = PerfilConst.Caixa + "," + PerfilConst.Gerente)]
    public class VendaController : ControllerBase
    {
        private readonly IVendaService _vendaService;

        public VendaController(IVendaService vendaService)
        {
            _vendaService = vendaService;
        }

        [HttpGet]
        [Authorize(Roles = PerfilConst.Gerente)]
        public async Task<IActionResult> BuscarTodos([FromQuery] PaginacaoParametros paginacao)
        {
            var resultado = await _vendaService.ListarAsync(paginacao);
            return Ok(resultado);
        }

        [HttpGet("{Id}")]
        [Authorize(Roles = PerfilConst.Gerente)]
        public async Task<IActionResult> BuscarPorId(int Id)
        {
            var resultado = await _vendaService.ObterPorIdAsync(Id);
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarVendaDto dto)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var resultado = await _vendaService.CriarAsync(dto, usuarioId);
            return Ok(resultado);
        }

    }
}

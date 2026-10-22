using Controle_De_estoqueAPI.Constantes;
using Controle_De_estoqueAPI.Dtos.Venda;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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
        public async Task<IActionResult> BuscarTodos()
        {
            var resultado = await _vendaService.ListarAsync();
            return Ok(resultado);
        }

        [HttpGet("{Id}")]
        [Authorize(Roles = PerfilConst.Gerente)]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var resultado = await _vendaService.ObterPorIdAsync(id);
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarVendaDto dto)
        {
            var resultado = await _vendaService.CriarAsync(dto);
            return Ok(resultado);
        }

    }
}

using Controle_De_estoqueAPI.Constantes;
using Controle_De_estoqueAPI.Dtos.Comum;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controle_De_estoqueAPI.Controllers
{
    /// <summary>
    /// Consultas de apoio para as telas do app desktop. Existem porque o Caixa precisa
    /// ver os produtos para vender e o Estoquista precisa listar fornecedores para
    /// cadastrar produtos, mas essas rotas originais são restritas a outros perfis.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = PerfilConst.Caixa + "," + PerfilConst.Estoquista + "," + PerfilConst.Gerente)]
    public class CatalogoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        private readonly IFornecedorService _fornecedorService;

        public CatalogoController(IProdutoService produtoService, IFornecedorService fornecedorService)
        {
            _produtoService = produtoService;
            _fornecedorService = fornecedorService;
        }

        /// <summary>Produtos ativos, paginados. Liberado para os três perfis.</summary>
        [HttpGet("produtos")]
        public async Task<IActionResult> Produtos([FromQuery] PaginacaoParametros paginacao)
            => Ok(await _produtoService.ListarAsync(paginacao, incluirInativos: false));

        /// <summary>Fornecedores. O [Authorize] do método soma ao da classe: só Estoquista e Gerente.</summary>
        [HttpGet("fornecedores")]
        [Authorize(Roles = PerfilConst.Estoquista + "," + PerfilConst.Gerente)]
        public async Task<IActionResult> Fornecedores()
            => Ok(await _fornecedorService.ListarAsync());
    }
}

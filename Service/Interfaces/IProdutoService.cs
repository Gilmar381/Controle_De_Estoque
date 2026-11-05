using Controle_De_estoqueAPI.Dtos.Comum;
using Controle_De_estoqueAPI.Dtos.Produto;

namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface IProdutoService
    {
        Task<ProdutoRespostaDto> CriarAsync(CriarProdutoDto dto);
        Task<ProdutoRespostaDto> ObterPorIdAsync(int id);
        Task<PaginadoDto<ProdutoRespostaDto>> ListarAsync(PaginacaoParametros paginacao, bool incluirInativos = false);
        Task<ProdutoRespostaDto> AtualizarAsync(int id, AtualizarProdutoDto dto);

        /// <summary>Exclusão lógica: desativa o produto, preservando vendas e histórico.</summary>
        Task<DeleterRespostaDto> ExcluirAsync(int id);
    }
}

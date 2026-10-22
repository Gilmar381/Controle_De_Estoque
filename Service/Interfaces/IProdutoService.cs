using Controle_De_estoqueAPI.Dtos.Produto;

namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface IProdutoService
    {
        Task<ProdutoRespostaDto> CriarAsync(CriarProdutoDto dto);
        Task<ProdutoRespostaDto> ObterPorIdAsync(int id);
        Task<IEnumerable<ProdutoRespostaDto>> ListarAsync();
        Task<ProdutoRespostaDto>AtualizarAsync(int id, AtualizarProdutoDto dto);
        Task <DeleterRespostaDto> ExcluirAsync(int id);
        
    }
}

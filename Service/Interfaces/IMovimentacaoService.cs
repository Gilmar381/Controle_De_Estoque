using Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque;
using Controle_De_estoqueAPI.Models;

namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface IMovimentacaoService
    {
        Task<MovimentacaoRespostaDto>RegistrarEntradaAsync( CriarMovimentacaoDto dto);
        Task<List<MovimentacaoSaidaItemRespostaDto>> RegistrarSaidaAsync(RegistrarSaidaDto dto);
        Task<IEnumerable<ListarTodosDto>>ListarAsync();    
        Task<ListarTodosDto>ObterPorIdAsync(int id);
        Task<IEnumerable<MovimentacaoRespostaDto>>ListarPorProdutoAsync(int produtoId);
        Task<IEnumerable<MovimentacaoRespostaDto>>ListarPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);

    }
}

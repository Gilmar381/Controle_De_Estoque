using Controle_De_estoqueAPI.Dtos.Comum;
using Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque;

namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface IMovimentacaoService
    {
        Task<MovimentacaoRespostaDto> RegistrarEntradaAsync(CriarMovimentacaoDto dto, int usuarioId);
        Task<List<MovimentacaoSaidaItemRespostaDto>> RegistrarSaidaAsync(RegistrarSaidaDto dto, int usuarioId);
        Task<PaginadoDto<ListarTodosDto>> ListarAsync(PaginacaoParametros paginacao);
        Task<MovimentacaoRespostaDto> ObterPorIdAsync(int id);
        Task<PaginadoDto<MovimentacaoRespostaDto>> ListarPorProdutoAsync(int produtoId, PaginacaoParametros paginacao);
        Task<PaginadoDto<MovimentacaoRespostaDto>> ListarPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, PaginacaoParametros paginacao);
    }
}

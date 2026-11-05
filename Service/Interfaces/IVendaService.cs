using Controle_De_estoqueAPI.Dtos.Comum;
using Controle_De_estoqueAPI.Dtos.Venda;

namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface IVendaService
    {
        Task<VendaRespostaDto> CriarAsync(CriarVendaDto dto, int usuarioId);
        Task<VendaRespostaDto> ObterPorIdAsync(int id);
        Task<PaginadoDto<VendaRespostaDto>> ListarAsync(PaginacaoParametros paginacao);
    }
}

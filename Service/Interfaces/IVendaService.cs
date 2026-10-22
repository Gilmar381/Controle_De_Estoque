using Controle_De_estoqueAPI.Dtos.Venda;
using Controle_De_estoqueAPI.Models;

namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface IVendaService
    {
        Task<VendaRespostaDto> CriarAsync(CriarVendaDto dto);
        Task<VendaRespostaDto> ObterPorIdAsync(int id);
        Task<IEnumerable<VendaRespostaDto>> ListarAsync();
        
        
    }
}

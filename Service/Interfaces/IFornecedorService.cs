using Controle_De_estoqueAPI.Dtos.Fornecedor;

namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface IFornecedorService
    {
        Task<FornecedorRespostaDto> CriarAsync(CriarFornecedorDto dto);

        Task<FornecedorRespostaDto>ObterPorIdAsync(int id);

        Task<IEnumerable<FornecedorRespostaDto>> ListarAsync();

        Task<FornecedorRespostaDto> AtualizarAsync(int Id, AtualizarFornecedorDto dto);
        Task ExcluirAsync(int id);
    }

    
}

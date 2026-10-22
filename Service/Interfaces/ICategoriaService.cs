using Controle_De_estoqueAPI.Dtos.Categoria;

namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface ICategoriaService
    {
        Task<CategoriaRespostaDto>CriarAsync(CriarCategoriaDto dto);
        Task<CategoriaRespostaDto>ObterPorIdAsync(int id);
        Task<IEnumerable<CategoriaRespostaDto>>ListarAsync();
        Task<CategoriaRespostaDto>AtualizarAsync(int id, AtualizarCategoriaDto dto);
        Task ExcluirAsync(int id);
        
    }
}

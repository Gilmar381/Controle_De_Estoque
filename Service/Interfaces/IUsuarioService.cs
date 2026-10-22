using Controle_De_estoqueAPI.Dtos.Usuario;

namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioRespostaDto> CriarAsync(CriarUsuarioDto dto);
        Task<UsuarioRespostaBuscaDto> ObterPorIdAsync(int id);
        Task<IEnumerable<UsuarioRespostaBuscaDto>> ListarAsync();
        Task<UsuarioRespostaDto> AtualizarAsync(int id, AtualizarUsuarioDto dto);
       
        Task<ExclusaoRespostaDto> ExcluirAsync(int id);
       
    }
}
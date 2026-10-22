using Controle_De_estoqueAPI.Dtos.Perfil;

namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface IPerfilService
    {
        Task<PerfilRespostaDto?> ObterPorIdAsync(int id);
        Task<IEnumerable<PerfilRespostaDto>> ListarAsync();//Não implementada e sem uso ate o momento
    }
}

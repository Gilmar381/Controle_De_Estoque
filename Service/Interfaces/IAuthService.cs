using Controle_De_estoqueAPI.Dtos.Usuario;

namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface IAuthService
    {
        Task<AlterarSenhaRespostaDto> AlterarSenhaAsync(int id, AlterarSenhaDto dto);
        Task<LoginRespostaDto> LoginAsync(LoginDto dto);
    }
}

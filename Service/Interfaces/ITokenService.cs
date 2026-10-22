using Controle_De_estoqueAPI.Models;

namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface ITokenService
    {
        public string GerarToken(Usuario usuario);      
    }
}

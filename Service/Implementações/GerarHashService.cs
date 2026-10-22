using BCrypt.Net;
using Controle_De_estoqueAPI.Service.Interfaces;

namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class GerarHashService : IGerarHashService
    {
        public string GerarHash(string senha) => BCrypt.Net.BCrypt.HashPassword(senha, workFactor: 10);
        public bool VerificarHash(string senha, string hash) => BCrypt.Net.BCrypt.Verify(senha, hash);
    }
}

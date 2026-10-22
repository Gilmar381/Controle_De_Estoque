namespace Controle_De_estoqueAPI.Service.Interfaces
{
    public interface IGerarHashService
    {
        string GerarHash(string senha);
        bool VerificarHash(string senha, string hash);
    }
}

namespace Controle_De_estoqueAPI.Dtos.Usuario
{
    public class AlterarSenhaRespostaDto
    {
        public bool Sucesso { get; set; }   = true;       // indica se a operação foi bem-sucedida
        public string? Mensagem { get; set; }      // mensagem amigável para o client (ex.: "Senha alterada com sucesso")
    }
}

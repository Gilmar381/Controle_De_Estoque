namespace Controle_De_estoqueAPI.Dtos.Usuario
{
    public class LoginRespostaDto
    {
        public bool Sucesso{ get; set; }
        public string Token { get; set; }
        public int UsuarioId { get; set; }
        public string Nome { get; set; }
        public string PerfilNome { get; set; }
        public string Mensagem { get; set; }
    }
}

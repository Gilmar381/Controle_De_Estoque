using Controle_De_estoqueAPI.Models;

namespace Controle_De_estoqueAPI.Dtos.Usuario
{
    public class UsuarioRespostaBuscaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public int PerfilId { get; set; }
        public bool Ativo {  get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime UltimoLogin { get; set; }
        public string PerfilNome { get; set; } 
    }
}

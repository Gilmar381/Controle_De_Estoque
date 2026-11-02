using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Usuario
{
    public class AtualizarUsuarioDto 
    {
       
        [MaxLength(100)]
        public  string? Nome { get; set; }

        
        [EmailAddress]
        public  string? Email { get; set; }

        public int PerfilId { get; set; }
        public bool Ativo { get; set; }
    }
}

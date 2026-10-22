using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Usuario
{
    public class AtualizarUsuarioDto 
    {
       
        [MaxLength(100)]
        public  string? Nome { get; set; }

        
        [EmailAddress]
        public  string? Email { get; set; }

        [Required(ErrorMessage = "O PerfilId é obrigatório.")]
        public int PerfilId { get; set; }
    }
}

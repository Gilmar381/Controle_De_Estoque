using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Controle_De_estoqueAPI.Models
{
    [Index(nameof(Login), IsUnique = true)]
    [Index(nameof(EmailUsuario), IsUnique = true)]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string NomeUsuario { get; set; }
        [Required]
        [EmailAddress]
        public string EmailUsuario { get; set; }

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9._]+$", ErrorMessage = "Login deve conter apenas letras, números, ponto ou underline.")]
        public string Login { get; set; }
        [Required]
        public string SenhaHash { get; set; }
      
        [Required]
        public int PerfilId { get; set; }
        [ForeignKey("PerfilId")]
        public Perfil Perfil { get; set; }
        [Required]
        public bool Ativo { get; set; } 
        [Required]
        public DateTime DataCriacao { get; set; }
        public DateTime UltimoLogin { get; set; }
      

    }
}

using System.ComponentModel.DataAnnotations;
using Controle_De_estoqueAPI.Constantes;

namespace Controle_De_estoqueAPI.Dtos.Usuario
{
    public class CriarUsuarioDto
    {

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Senha { get; set; }

        [Required]
        [MaxLength(100)]
        public string Login { get; set; }
        public int PerfilId { get; set; }
        


    }
}

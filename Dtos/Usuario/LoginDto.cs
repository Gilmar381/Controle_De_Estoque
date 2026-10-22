using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Usuario
{
    public class LoginDto
    {
        [Required]
        public string Senha { get; set; }

        [Required]
        public string Login { get; set; }
    }
}

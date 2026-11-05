using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Usuario
{
    public class CriarUsuarioDto
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        // 72 é o limite útil do BCrypt.
        [Required]
        [MinLength(8)]
        [MaxLength(72)]
        public string Senha { get; set; }

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9._]+$", ErrorMessage = "Login deve conter apenas letras, números, ponto ou underline.")]
        public string Login { get; set; }

        [Range(1, int.MaxValue)]
        public int PerfilId { get; set; }

        public bool Ativo { get; set; } = true;
    }
}

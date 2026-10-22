using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Usuario
{
    public class AlterarSenhaDto
    {
        [Required]
        [MaxLength(6)]
        public string SenhaAtual { get; set; }

        [Required]
        [MaxLength(6)]
        public string NovaSenha { get; set; }
    }
}

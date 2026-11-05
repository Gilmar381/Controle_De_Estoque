using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Usuario
{
    public class AlterarSenhaDto
    {
        // Sem MinLength: contas antigas podem ter senhas menores que a regra atual.
        [Required]
        [MaxLength(72)]
        public string SenhaAtual { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(72)]
        public string NovaSenha { get; set; }
    }
}

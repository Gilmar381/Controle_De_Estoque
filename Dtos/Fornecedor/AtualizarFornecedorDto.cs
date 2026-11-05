using Controle_De_estoqueAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Fornecedor
{
    public class AtualizarFornecedorDto
    {
        [Required]
        [MaxLength(150)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(18)]
        [CnpjValido]
        public string Cnpj { get; set; }

        [Required]
        [MaxLength(100)]
        [TelefoneValido]
        public string Telefone { get; set; }

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(300)]
        [Required]
        public string Endereco { get; set; }
    }
}

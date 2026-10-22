using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Controle_De_estoqueAPI.Models
{
    public class Fornecedor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string NomeFornecedor { get; set; }

        [Required]
        [MaxLength(18)]
        [CnpjValido]
        public string CNPJFornecedor { get; set; } 

        [Required]
        [MaxLength(100)]
        [TelefoneValido]
        public string TelefoneFornecedor { get; set; }

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string EmailFornecedor { get; set; }

        [MaxLength(300)]
        [Required]
        public string EnderecoFornecedor { get; set; }

        // Relacionamento: um fornecedor pode fornecer vários produtos
        public ICollection<Produto> Produtos { get; set; }
    }
}

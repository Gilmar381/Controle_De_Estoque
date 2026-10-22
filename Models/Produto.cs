using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Controle_De_estoqueAPI.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string NomeProduto { get; set; }
        [MaxLength(200)]
        public string DescricaoProduto { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }

        [Required]
        public int FornecedorId { get; set; }

        [ForeignKey("FornecedorId")]
        public Fornecedor Fornecedor { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoProduto { get; set; }

        public void DefinirPreco(decimal preco)
        {
            if (preco <= 0)
                throw new ArgumentException("O preço deve ser maior que zero.");
            PrecoProduto = preco;
        }
        [Required]
        public int QuantidadeEstoque { get; set; }
        [Required]
        public bool Ativo { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DataCadastro { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UltimaAtualizacao { get; set; }
        
      
    }
}

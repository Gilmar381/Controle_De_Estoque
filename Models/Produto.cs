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

        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }

        public int FornecedorId { get; set; }

        [ForeignKey("FornecedorId")]
        public Fornecedor Fornecedor { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço do produto deve ser maior que zero.")]
        public decimal PrecoProduto { get; set; }

        public int QuantidadeEstoque { get; set; }
        public bool Ativo { get; set; } = true;

        [DataType(DataType.DateTime)]
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime UltimaAtualizacao { get; set; }

        /// <summary>Controle de concorrência otimista: impede duas operações simultâneas sobrescreverem o estoque.</summary>
        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}

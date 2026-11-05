using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Controle_De_estoqueAPI.Models
{
    public class MovimentacaoEstoque
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public Produto Produto { get; set; }

        [Required]
        public TipoMovimentacao TipoMovimentacao { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DataMovimentacao { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        /// <summary>Venda que originou a movimentação (nula em entradas e saídas avulsas).</summary>
        public int? VendaId { get; set; }

        [ForeignKey("VendaId")]
        public Venda? Venda { get; set; }

        [Required]
        [MaxLength(200)]
        public string Observacao { get; set; }
    }

    // Enum para definir se é entrada ou saída
    public enum TipoMovimentacao
    {
        Entrada = 1,
        Saida = 2,
        Ajuste = 3
    }
}

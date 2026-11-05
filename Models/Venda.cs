using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Controle_De_estoqueAPI.Models
{
    public class Venda
    {
        [Key]
        public int Id { get; set; }

        public DateTime DataVenda { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalVenda { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public ICollection<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();
    }

    public enum FormaPagamento
    {
        Dinheiro = 1,
        CartaoCredito = 2,
        CartaoDebito = 3,
        Pix = 4,
        ValeAlimentacao = 5,
        Outros = 6
    }
}

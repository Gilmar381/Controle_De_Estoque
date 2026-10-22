
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
        public decimal TotalVenda { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public ICollection<ItemVenda> ItensVenda { get; set; }
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

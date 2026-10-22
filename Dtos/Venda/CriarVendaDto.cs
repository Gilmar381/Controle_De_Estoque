using Controle_De_estoqueAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Venda
{
    public class CriarVendaDto
    {
        [Required]
        public DateTime DataVenda { get; set; }
        public  int UsuarioId { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public ICollection<ItemVendaDto> ItensVenda { get; set; }

    }
}

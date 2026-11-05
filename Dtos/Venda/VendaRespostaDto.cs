using Controle_De_estoqueAPI.Models;

namespace Controle_De_estoqueAPI.Dtos.Venda
{
    public class VendaRespostaDto
    {
        public int Id { get; set; }
        public DateTime DataVenda { get; set; }
        public int UsuarioId { get; set; }
        public decimal TotalVenda { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public ICollection<ItemVendaRespostaDto> ItensVenda { get; set; } = new List<ItemVendaRespostaDto>();
    }
}

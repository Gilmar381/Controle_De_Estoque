using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Venda
{
    public class ItemVendaDto
    {
        [Range(1, int.MaxValue)]
        public int ProdutoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }
    }
}

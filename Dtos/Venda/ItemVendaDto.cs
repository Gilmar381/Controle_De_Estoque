using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Controle_De_estoqueAPI.Models;
namespace Controle_De_estoqueAPI.Dtos.Venda
{
    public class ItemVendaDto
    {

      
        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public int Quantidade { get; set; }

    }
}

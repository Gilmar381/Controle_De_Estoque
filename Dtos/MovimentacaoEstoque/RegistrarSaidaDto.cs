using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque
{
    public class RegistrarSaidaDto
    {
        /// <summary>Venda que originou a saída. Se nula, é uma saída avulsa e a Observacao é obrigatória.</summary>
        public int? VendaId { get; set; }

        [MaxLength(200)]
        public string? Observacao { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Informe ao menos um item.")]
        public List<ItemSaidaDto> Itens { get; set; } = new();
    }

    public class ItemSaidaDto
    {
        [Range(1, int.MaxValue)]
        public int ProdutoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }
    }
}

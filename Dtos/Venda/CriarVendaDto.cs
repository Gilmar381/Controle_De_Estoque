using Controle_De_estoqueAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Venda
{
    /// <summary>
    /// Data e usuário NÃO vêm do cliente: a data é gerada pelo servidor (UTC)
    /// e o usuário é o autenticado no token JWT.
    /// </summary>
    public class CriarVendaDto
    {
        [EnumDataType(typeof(FormaPagamento))]
        public FormaPagamento FormaPagamento { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "A venda deve conter pelo menos um item.")]
        public List<ItemVendaDto> ItensVenda { get; set; } = new();
    }
}

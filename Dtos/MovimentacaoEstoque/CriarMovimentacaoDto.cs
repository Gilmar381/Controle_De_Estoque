using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque
{
    /// <summary>Entrada de estoque. O usuário responsável vem do token JWT, não do corpo.</summary>
    public class CriarMovimentacaoDto
    {
        [Range(1, int.MaxValue)]
        public int ProdutoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        [Required]
        [MaxLength(200)]
        public string Observacao { get; set; }
    }
}

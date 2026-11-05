using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Produto
{
    /// <summary>Atualização parcial: só os campos enviados (não nulos) são alterados.</summary>
    public class AtualizarProdutoDto
    {
        [MaxLength(100)]
        public string? NomeProduto { get; set; }

        [MaxLength(200)]
        public string? DescricaoProduto { get; set; }

        [Range(1, int.MaxValue)]
        public int? CategoriaId { get; set; }

        [Range(1, int.MaxValue)]
        public int? FornecedorId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O preço do produto deve ser maior que zero.")]
        public decimal? PrecoProduto { get; set; }

        public bool? Ativo { get; set; }
    }
}

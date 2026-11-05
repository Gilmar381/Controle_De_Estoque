using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Produto
{
    public class CriarProdutoDto
    {
        [Required]
        [MaxLength(100)]
        public string NomeProduto { get; set; }

        [Required]
        [MaxLength(200)]
        public string DescricaoProduto { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoriaId { get; set; }

        [Range(1, int.MaxValue)]
        public int FornecedorId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O preço do produto deve ser maior que zero.")]
        public decimal PrecoProduto { get; set; }
    }
}

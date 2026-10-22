using Controle_De_estoqueAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        public int CategoriaId {  get; set; }

        [Required]      
        public int FornecedorId { get; set; }

        [Required]
        public decimal PrecoProduto { get; set; }
    }
}

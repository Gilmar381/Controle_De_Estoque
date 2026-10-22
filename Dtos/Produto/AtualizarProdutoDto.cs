using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Produto
{
    // Herda a estrutura, mas flexibiliza os campos para alteração parcial
    public class AtualizarProdutoDto 
    {
        [MaxLength(100)]
        public  string? NomeProduto { get; set; }

        [MaxLength(200)]
        public  string? NomeDescricao { get; set; }

        [MaxLength(100)]
        public int CategoriaId { get; set; } // Mantive o "i" extra igual ao seu original

        [MaxLength(200)]
        public int FornecedorId { get; set; }

        // O tipo 'decimal?' permite que o preço venha nulo caso não seja alterado
        public  decimal? PrecoProduto { get; set; }
        public string DescricaoProduto { get; internal set; }
        public bool Ativo { get; internal set; }
    }
}

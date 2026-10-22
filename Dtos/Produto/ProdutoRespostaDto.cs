using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Produto
{
    public class ProdutoRespostaDto
    {
       public int Id { get; set; }
       public string NomeProduto { get; set; }
       public string DescricaoProduto{ get; set; }
       public decimal PrecoProduto { get; set; }
       public string NomeFornecedor { get; set; }
        public string NomeCategoria { get; set; }
        public int QuantidadeEstoque { get; set; }
        
        public bool Ativo { get; set; }
       
        [DataType(DataType.DateTime)]
        public DateTime DataCadastro { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime UltimaAtualizacao { get; set; }

    }
}

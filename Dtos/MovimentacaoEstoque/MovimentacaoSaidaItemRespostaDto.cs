namespace Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque
{
    public class MovimentacaoSaidaItemRespostaDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public string Observacao { get; set; }
    }
}
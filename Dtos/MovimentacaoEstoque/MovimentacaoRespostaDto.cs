using Controle_De_estoqueAPI.Models;

namespace Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque
{
    public class MovimentacaoRespostaDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public TipoMovimentacao TipoMovimentacao { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
        public string Observacao { get; set; }
    }
}

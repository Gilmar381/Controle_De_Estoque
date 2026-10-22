using Controle_De_estoqueAPI.Models;

namespace Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque
{
    public class ListarTodosDto
    {
        public DateTime DataMovimentacao { get; set; }
        public int Id { get; set; }
        public TipoMovimentacao TipoMovimentacao { get; set; }
    }
}

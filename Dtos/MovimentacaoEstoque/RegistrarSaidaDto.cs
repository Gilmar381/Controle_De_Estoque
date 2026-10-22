namespace Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque
{
    public class RegistrarSaidaDto
    {
        public int VendaId { get; set; }
        public List<ItemSaidaDto> Itens { get; set; } = new();
    }

    public class ItemSaidaDto
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}

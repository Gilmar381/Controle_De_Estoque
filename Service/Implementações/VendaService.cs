using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque;
using Controle_De_estoqueAPI.Dtos.Venda;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class VendaService : IVendaService
    {
        private readonly ControleEstoqueDbContexto _context;
        private readonly IMovimentacaoService _movimentacaoService;

        public VendaService(ControleEstoqueDbContexto context, IMovimentacaoService movimentacao)
        {
            _context = context;
            _movimentacaoService = movimentacao;
            
        }

        public async Task<VendaRespostaDto> CriarAsync(CriarVendaDto dto, int usuarioId)
        {
            if (dto.ItensVenda == null || !dto.ItensVenda.Any())
                throw new Exception("A venda deve conter pelo menos um item.");

            // Agrupa itens repetidos do mesmo produto
            var itensAgrupados = dto.ItensVenda
                .GroupBy(i => i.ProdutoId)
                .Select(g => new { ProdutoId = g.Key, Quantidade = g.Sum(x => x.Quantidade) })
                .ToList();

            var produtosIds = itensAgrupados.Select(i => i.ProdutoId).ToList();

            var produtos = await _context.Produtos
                .Where(p => produtosIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            if (produtos.Count != produtosIds.Count)
                throw new Exception("Um ou mais produtos não existem.");

            foreach (var item in itensAgrupados)
            {
                var produto = produtos[item.ProdutoId];

                if (item.Quantidade <= 0)
                    throw new Exception("A quantidade deve ser maior que zero.");

                if (!produto.Ativo)
                    throw new Exception($"O produto '{produto.NomeProduto}' está inativo.");

                if (produto.QuantidadeEstoque < item.Quantidade)
                    throw new Exception($"Estoque insuficiente para '{produto.NomeProduto}'.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var venda = new Venda
                {
                    DataVenda = DateTime.UtcNow,
                    UsuarioId = dto.UsuarioId,
                    FormaPagamento = dto.FormaPagamento,
                    ItensVenda = itensAgrupados.Select(i => new ItemVenda
                    {
                        ProdutoId = i.ProdutoId,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = produtos[i.ProdutoId].PrecoProduto
                    }).ToList()
                };

                venda.TotalVenda = venda.ItensVenda.Sum(i => i.Quantidade * i.PrecoUnitario);

                _context.Vendas.Add(venda);
                await _context.SaveChangesAsync();

                var saidaDto = new RegistrarSaidaDto
                {
                    VendaId = venda.Id,
                    Itens = venda.ItensVenda.Select(i => new ItemSaidaDto
                    {
                        ProdutoId = i.ProdutoId,
                        Quantidade = i.Quantidade
                    }).ToList()
                };

                await _movimentacaoService.RegistrarSaidaAsync(saidaDto, dto.UsuarioId);

                await transaction.CommitAsync();

                return new VendaRespostaDto
                {
                    Id = venda.Id,
                    DataVenda = venda.DataVenda,
                    UsuarioId = venda.UsuarioId,
                    TotalVenda = venda.TotalVenda,
                    FormaPagamento = venda.FormaPagamento,
                    ItensVenda = venda.ItensVenda.Select(i => new ItemVendaDto
                    {
                        ProdutoId = i.ProdutoId,
                        Quantidade = i.Quantidade
                    }).ToList()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<VendaRespostaDto> ObterPorIdAsync(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.ItensVenda)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (venda == null)
            {
                throw new Exception("Venda não encontrada.");
            }
            return new VendaRespostaDto
            {
                Id = venda.Id,
                DataVenda = venda.DataVenda,
                UsuarioId = venda.UsuarioId,
                TotalVenda = venda.TotalVenda,
                FormaPagamento = venda.FormaPagamento,
                ItensVenda = venda.ItensVenda.Select(i => new ItemVendaDto
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList()
            };
        }

        public async Task<IEnumerable<VendaRespostaDto>> ListarAsync()
        {
            var vendas = await _context.Vendas
                .Include(v => v.ItensVenda)
                .ToListAsync();
            return vendas.Select(v => new VendaRespostaDto
            {
                Id = v.Id,
                DataVenda = v.DataVenda,
                UsuarioId = v.UsuarioId,
                TotalVenda = v.TotalVenda,
                FormaPagamento = v.FormaPagamento,
                ItensVenda = v.ItensVenda.Select(i => new ItemVendaDto
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                }).ToList()
            });
        }

        public async Task<decimal> CalcularTotalAsync(List<ItemVendaDto> dto)
        {
            var produtosIds = dto.Select(i => i.ProdutoId).ToList();
            var produtos = await _context.Produtos
                .Where(p => produtosIds.Contains(p.Id))
                .ToListAsync();
           if (produtos.Count != dto.Count)
            {
                throw new Exception("Um ou mais produtos não existem.");
            }
             decimal total = 0;
            foreach (var item in dto)
            {
                var produto = produtos.First(p => p.Id == item.ProdutoId);
                total += item.Quantidade * produto.PrecoProduto; // usa o preço oficial do estoque
            }

            return total;
        }

    }


   

    

  

   
   
       
       
   
    
   
} 


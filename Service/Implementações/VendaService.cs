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

        public async Task<VendaRespostaDto> CriarAsync(CriarVendaDto dto)// Parei exatamente aqui, mas posso continuar a implementação do método CriarAsync para você. Aqui está a continuação do código:
        {
            var produtosIds = dto.ItensVenda.Select(iv => iv.ProdutoId).ToList();
            var produto = await _context.Produtos
                    .Where(p => produtosIds.Contains(p.Id))
                    .ToListAsync();
            if (produto.Count != dto.ItensVenda.Count)
            {
                throw new Exception("Um ou mais produtos não existem.");
            }
            var venda = new Venda
            {
                DataVenda = DateTime.Now,
                UsuarioId = dto.UsuarioId,
               
                TotalVenda = 0, // Inicialmente zero, será calculado depois
                FormaPagamento = dto.FormaPagamento,
                
               ItensVenda = dto.ItensVenda
               .Select(i => new ItemVenda
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = produto.First(p => p. Id == i.ProdutoId).PrecoProduto
                }).ToList()
            };
            // Calcula o total da venda
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

            await _movimentacaoService.RegistrarSaidaAsync(saidaDto);

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
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()

            };
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
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
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
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario
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


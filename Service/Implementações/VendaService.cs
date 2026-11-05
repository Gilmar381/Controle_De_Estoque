using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.Comum;
using Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque;
using Controle_De_estoqueAPI.Dtos.Venda;
using Controle_De_estoqueAPI.Exceptions;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        private static readonly Expression<Func<Venda, VendaRespostaDto>> Projecao = v => new VendaRespostaDto
        {
            Id = v.Id,
            DataVenda = v.DataVenda,
            UsuarioId = v.UsuarioId,
            TotalVenda = v.TotalVenda,
            FormaPagamento = v.FormaPagamento,
            ItensVenda = v.ItensVenda.Select(i => new ItemVendaRespostaDto
            {
                Id = i.Id,
                VendaId = i.VendaId,
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                Subtotal = i.Subtotal
            }).ToList()
        };

        // Versão compilada da projeção, para mapear entidades já em memória.
        private static readonly Func<Venda, VendaRespostaDto> ParaDto = Projecao.Compile();

        public async Task<VendaRespostaDto> CriarAsync(CriarVendaDto dto, int usuarioId)
        {
            if (dto.ItensVenda == null || dto.ItensVenda.Count == 0)
                throw new RegraDeNegocioException("A venda deve conter pelo menos um item.");

            // Agrupa itens repetidos do mesmo produto
            var itensAgrupados = dto.ItensVenda
                .GroupBy(i => i.ProdutoId)
                .Select(g => new { ProdutoId = g.Key, Quantidade = g.Sum(x => x.Quantidade) })
                .ToList();

            var produtosIds = itensAgrupados.Select(i => i.ProdutoId).ToList();

            var produtos = await _context.Produtos
                .Where(p => produtosIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var faltando = produtosIds.Where(id => !produtos.ContainsKey(id)).ToList();
            if (faltando.Count > 0)
                throw new KeyNotFoundException($"Produto(s) não encontrado(s): {string.Join(", ", faltando)}.");

            foreach (var item in itensAgrupados)
            {
                var produto = produtos[item.ProdutoId];

                if (item.Quantidade <= 0)
                    throw new RegraDeNegocioException("A quantidade deve ser maior que zero.");

                if (!produto.Ativo)
                    throw new RegraDeNegocioException($"O produto '{produto.NomeProduto}' está inativo.");

                if (produto.QuantidadeEstoque < item.Quantidade)
                    throw new RegraDeNegocioException($"Estoque insuficiente para '{produto.NomeProduto}'.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var venda = new Venda
                {
                    DataVenda = DateTime.UtcNow,
                    UsuarioId = usuarioId, // sempre o usuário autenticado
                    FormaPagamento = dto.FormaPagamento,
                    ItensVenda = itensAgrupados.Select(i =>
                    {
                        var preco = produtos[i.ProdutoId].PrecoProduto; // preço oficial do cadastro
                        return new ItemVenda
                        {
                            ProdutoId = i.ProdutoId,
                            Quantidade = i.Quantidade,
                            PrecoUnitario = preco,
                            Subtotal = i.Quantidade * preco
                        };
                    }).ToList()
                };

                venda.TotalVenda = venda.ItensVenda.Sum(i => i.Subtotal);

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

                await _movimentacaoService.RegistrarSaidaAsync(saidaDto, usuarioId);

                await transaction.CommitAsync();

                return ParaDto(venda);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<VendaRespostaDto> ObterPorIdAsync(int id)
        {
            return await _context.Vendas
                .AsNoTracking()
                .Where(v => v.Id == id)
                .Select(Projecao)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Venda não encontrada.");
        }

        public async Task<PaginadoDto<VendaRespostaDto>> ListarAsync(PaginacaoParametros paginacao)
        {
            return await _context.Vendas
                .AsNoTracking()
                .OrderByDescending(v => v.DataVenda).ThenByDescending(v => v.Id)
                .Select(Projecao)
                .ToPaginadoAsync(paginacao);
        }
    }
}
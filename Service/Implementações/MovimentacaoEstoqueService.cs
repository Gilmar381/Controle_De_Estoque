using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.Comum;
using Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque;
using Controle_De_estoqueAPI.Exceptions;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class MovimentacaoEstoqueService : IMovimentacaoService
    {
        private readonly ControleEstoqueDbContexto _context;

        public MovimentacaoEstoqueService(ControleEstoqueDbContexto context)
        {
            _context = context;
        }

        private static readonly Expression<Func<MovimentacaoEstoque, MovimentacaoRespostaDto>> Projecao =
            m => new MovimentacaoRespostaDto
            {
                Id = m.Id,
                ProdutoId = m.ProdutoId,
                ProdutoNome = m.Produto.NomeProduto,
                TipoMovimentacao = m.TipoMovimentacao,
                Quantidade = m.Quantidade,
                DataMovimentacao = m.DataMovimentacao,
                UsuarioId = m.UsuarioId,
                UsuarioNome = m.Usuario!.NomeUsuario,
                Observacao = m.Observacao
            };

        public async Task<MovimentacaoRespostaDto> RegistrarEntradaAsync(CriarMovimentacaoDto dto, int usuarioId)
        {
            var produto = await _context.Produtos.FindAsync(dto.ProdutoId)
                ?? throw new KeyNotFoundException($"Produto {dto.ProdutoId} não encontrado.");

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == usuarioId)
                .Select(u => new { u.Id, u.NomeUsuario })
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException($"Usuário {usuarioId} não encontrado.");

            var agora = DateTime.UtcNow;

            produto.QuantidadeEstoque += dto.Quantidade;
            produto.UltimaAtualizacao = agora;

            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoId = produto.Id,
                UsuarioId = usuario.Id,
                Quantidade = dto.Quantidade,
                TipoMovimentacao = TipoMovimentacao.Entrada,
                DataMovimentacao = agora,
                Observacao = dto.Observacao
            };

            _context.MovimentacaoEstoques.Add(movimentacao);

            // Um único SaveChanges: estoque e movimentação são gravados juntos ou não são gravados.
            await _context.SaveChangesAsync();

            return new MovimentacaoRespostaDto
            {
                Id = movimentacao.Id,
                ProdutoId = produto.Id,
                ProdutoNome = produto.NomeProduto,
                TipoMovimentacao = movimentacao.TipoMovimentacao,
                Quantidade = movimentacao.Quantidade,
                DataMovimentacao = movimentacao.DataMovimentacao,
                UsuarioId = usuario.Id,
                UsuarioNome = usuario.NomeUsuario,
                Observacao = movimentacao.Observacao
            };
        }

        public async Task<List<MovimentacaoSaidaItemRespostaDto>> RegistrarSaidaAsync(RegistrarSaidaDto dto, int usuarioId)
        {
            if (dto.Itens == null || dto.Itens.Count == 0)
                throw new RegraDeNegocioException("Informe ao menos um item.");

            if (dto.Itens.Any(i => i.Quantidade <= 0))
                throw new RegraDeNegocioException("A quantidade de cada item deve ser maior que zero.");

            if (dto.VendaId is null && string.IsNullOrWhiteSpace(dto.Observacao))
                throw new RegraDeNegocioException("Em saídas avulsas (sem venda), a observação é obrigatória.");

            if (dto.VendaId is int vendaId)
            {
                if (!await _context.Vendas.AnyAsync(v => v.Id == vendaId))
                    throw new KeyNotFoundException($"Venda {vendaId} não encontrada.");

                // Evita baixar o estoque duas vezes para a mesma venda.
                if (await _context.MovimentacaoEstoques.AnyAsync(m =>
                        m.VendaId == vendaId && m.TipoMovimentacao == TipoMovimentacao.Saida))
                    throw new ConflitoException($"Já existe baixa de estoque registrada para a venda {vendaId}.");
            }

            // Agrupa itens repetidos do mesmo produto
            var itens = dto.Itens
                .GroupBy(i => i.ProdutoId)
                .Select(g => new { ProdutoId = g.Key, Quantidade = g.Sum(x => x.Quantidade) })
                .ToList();

            var ids = itens.Select(i => i.ProdutoId).ToList();
            var produtos = await _context.Produtos
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var faltando = ids.Where(id => !produtos.ContainsKey(id)).ToList();
            if (faltando.Count > 0)
                throw new KeyNotFoundException($"Produto(s) não encontrado(s): {string.Join(", ", faltando)}.");

            // 1ª passada: valida tudo antes de alterar qualquer coisa
            foreach (var item in itens)
            {
                var produto = produtos[item.ProdutoId];
                if (produto.QuantidadeEstoque < item.Quantidade)
                    throw new RegraDeNegocioException($"Estoque insuficiente para o produto '{produto.NomeProduto}'.");
            }

            // 2ª passada: aplica
            var agora = DateTime.UtcNow;
            var observacao = dto.VendaId.HasValue
                ? $"Saída referente à venda {dto.VendaId}"
                : dto.Observacao!;

            var criadas = new List<(MovimentacaoEstoque Mov, string ProdutoNome)>();

            foreach (var item in itens)
            {
                var produto = produtos[item.ProdutoId];
                produto.QuantidadeEstoque -= item.Quantidade;
                produto.UltimaAtualizacao = agora;

                var mov = new MovimentacaoEstoque
                {
                    ProdutoId = produto.Id,
                    UsuarioId = usuarioId,
                    Quantidade = item.Quantidade,
                    TipoMovimentacao = TipoMovimentacao.Saida,
                    DataMovimentacao = agora,
                    VendaId = dto.VendaId,
                    Observacao = observacao
                };

                _context.MovimentacaoEstoques.Add(mov);
                criadas.Add((mov, produto.NomeProduto));
            }

            // Um único SaveChanges = tudo ou nada. Se outra operação alterou o estoque
            // no meio tempo, o RowVersion do Produto causa DbUpdateConcurrencyException (HTTP 409).
            await _context.SaveChangesAsync();

            return criadas.Select(x => new MovimentacaoSaidaItemRespostaDto
            {
                Id = x.Mov.Id,
                ProdutoId = x.Mov.ProdutoId,
                ProdutoNome = x.ProdutoNome,
                Quantidade = x.Mov.Quantidade,
                DataMovimentacao = x.Mov.DataMovimentacao,
                Observacao = x.Mov.Observacao
            }).ToList();
        }

        public async Task<PaginadoDto<ListarTodosDto>> ListarAsync(PaginacaoParametros paginacao)
        {
            return await _context.MovimentacaoEstoques
                .AsNoTracking()
                .OrderByDescending(m => m.DataMovimentacao).ThenByDescending(m => m.Id)
                .Select(m => new ListarTodosDto
                {
                    Id = m.Id,
                    DataMovimentacao = m.DataMovimentacao,
                    TipoMovimentacao = m.TipoMovimentacao
                })
                .ToPaginadoAsync(paginacao);
        }

        public async Task<MovimentacaoRespostaDto> ObterPorIdAsync(int id)
        {
            return await _context.MovimentacaoEstoques
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(Projecao)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException($"Movimentação com ID {id} não encontrada.");
        }

        public async Task<PaginadoDto<MovimentacaoRespostaDto>> ListarPorProdutoAsync(int produtoId, PaginacaoParametros paginacao)
        {
            if (!await _context.Produtos.AnyAsync(p => p.Id == produtoId))
                throw new KeyNotFoundException($"Produto com ID {produtoId} não encontrado.");

            return await _context.MovimentacaoEstoques
                .AsNoTracking()
                .Where(m => m.ProdutoId == produtoId)
                .OrderByDescending(m => m.DataMovimentacao).ThenByDescending(m => m.Id) // mais recentes primeiro
                .Select(Projecao)
                .ToPaginadoAsync(paginacao);
        }

        public async Task<PaginadoDto<MovimentacaoRespostaDto>> ListarPorPeriodoAsync(
            DateTime dataInicio, DateTime dataFim, PaginacaoParametros paginacao)
        {
            if (dataFim.Date < dataInicio.Date)
                throw new RegraDeNegocioException("A data final não pode ser anterior à data inicial.");

            // Intervalo por dia inteiro: [inicio 00:00, dia seguinte ao fim 00:00).
            // Assim o último dia informado entra completo na consulta (datas em UTC).
            var inicio = dataInicio.Date;
            var fimExclusivo = dataFim.Date.AddDays(1);

            return await _context.MovimentacaoEstoques
                .AsNoTracking()
                .Where(m => m.DataMovimentacao >= inicio && m.DataMovimentacao < fimExclusivo)
                .OrderByDescending(m => m.DataMovimentacao).ThenByDescending(m => m.Id)
                .Select(Projecao)
                .ToPaginadoAsync(paginacao);
        }
    }
}
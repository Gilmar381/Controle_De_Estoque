using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Interfaces;
using Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;




namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class MovimentacaoEstoqueService : IMovimentacaoService
    {
        private readonly ControleEstoqueDbContexto _context;
        private readonly IProdutoService _produtoService;
        private readonly IUsuarioService _usuarioService;


        public MovimentacaoEstoqueService(ControleEstoqueDbContexto context,
            IProdutoService produto,
            IUsuarioService usuario
           )
        {
            _context = context;
            _produtoService = produto;
            _usuarioService = usuario;
            
        }

        public async Task<MovimentacaoRespostaDto> RegistrarEntradaAsync(CriarMovimentacaoDto dto)
        {

            var produto = await _produtoService.ObterPorIdAsync(dto.ProdutoId);
            if (produto == null)
            {
                throw new Exception("Produto não encontrado.");
            }
            var usuario = await _usuarioService.ObterPorIdAsync(dto.UsuarioId);
            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

           
            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoId = dto.ProdutoId,
                UsuarioId = usuario.Id,
                Quantidade = dto.Quantidade,
                TipoMovimentacao = TipoMovimentacao.Entrada,
                DataMovimentacao = DateTime.Now,
                Observacao = dto.Observacao
                
            };

            AtualizarEstoqueAsync(produto.Id, produto.QuantidadeEstoque + dto.Quantidade).Wait();


            _context.MovimentacaoEstoques.Add(movimentacao);
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
                UsuarioNome = usuario.PerfilNome,
                Observacao = movimentacao.Observacao
            };

        }

        public async Task<List<MovimentacaoSaidaItemRespostaDto>> RegistrarSaidaAsync(RegistrarSaidaDto dto, int usuarioId)
        {
            var movimentacoesCriadas = new List<(MovimentacaoEstoque Movimentacao, string ProdutoNome)>();

            foreach (var item in dto.Itens)
            {
                var produto = await _context.Produtos.FindAsync(item.ProdutoId);

                if (produto == null)
                    throw new KeyNotFoundException($"Produto {item.ProdutoId} não encontrado.");

                if (produto.QuantidadeEstoque < item.Quantidade)
                    throw new InvalidOperationException($"Estoque insuficiente para o produto {produto.NomeProduto}.");

                await AtualizarEstoqueAsync(produto.Id, produto.QuantidadeEstoque - item.Quantidade);

                var movimentacao = new MovimentacaoEstoque
                {
                    ProdutoId = item.ProdutoId,
                    UsuarioId =usuarioId,
                    Quantidade = item.Quantidade,
                    TipoMovimentacao = TipoMovimentacao.Saida,
                    DataMovimentacao = DateTime.Now,
                    Observacao = $"Saída referente à venda {dto.VendaId}"
                };

                _context.MovimentacaoEstoques.Add(movimentacao);
                movimentacoesCriadas.Add((movimentacao, produto.NomeProduto)); // guarda nome junto, evita buscar de novo
            }

            await _context.SaveChangesAsync(); // gera os Ids

            return movimentacoesCriadas.Select(x => new MovimentacaoSaidaItemRespostaDto
            {
                Id = x.Movimentacao.Id,
                ProdutoId = x.Movimentacao.ProdutoId,
                ProdutoNome = x.ProdutoNome,
                Quantidade = x.Movimentacao.Quantidade,
                DataMovimentacao = x.Movimentacao.DataMovimentacao,
                Observacao = x.Movimentacao.Observacao
            }).ToList();
        }

        public async Task<IEnumerable<ListarTodosDto>> ListarAsync()
        {
            var movimentacao = await _context.MovimentacaoEstoques
                .ToListAsync();
            return movimentacao.Select(R => new ListarTodosDto
            {
                Id = R.Id,
                DataMovimentacao = R.DataMovimentacao,
                TipoMovimentacao = R.TipoMovimentacao
            });
        }
        public async Task<ListarTodosDto> ObterPorIdAsync(int id)
        {
            var movimentacao = await _context.MovimentacaoEstoques.FindAsync(id);

            if (movimentacao == null)
            {
                throw new KeyNotFoundException($"Movimentação com ID {id} não encontrado.");

            }

            return new ListarTodosDto
            {
                Id = id,
                DataMovimentacao = movimentacao.DataMovimentacao,
                TipoMovimentacao = movimentacao.TipoMovimentacao
            };
        }

        public async Task<IEnumerable<MovimentacaoRespostaDto>> ListarPorProdutoAsync(int produtoId)
        {
            var movimentacao = await _context.MovimentacaoEstoques
                .Where(m => m.ProdutoId == produtoId)
                .OrderByDescending(m => m.DataMovimentacao) // mais recentes primeiro
                .ToListAsync();

            return movimentacao.Select(m => new MovimentacaoRespostaDto
            {
                UsuarioNome = _context.Usuarios.Where(u => u.Id == m.UsuarioId).Select(u => u.NomeUsuario).FirstOrDefault() ?? "Usuário não encontrado",
                ProdutoNome = _context.Produtos.Where(p => p.Id == m.ProdutoId).Select(p => p.NomeProduto).FirstOrDefault() ?? "Produto não encontrado",
                Id = m.Id,
                ProdutoId = m.ProdutoId,
                Quantidade = m.Quantidade,
                TipoMovimentacao = m.TipoMovimentacao,
                DataMovimentacao = m.DataMovimentacao,
                UsuarioId = m.UsuarioId,
                Observacao = m.Observacao,
            });
        }

        public async Task<IEnumerable<MovimentacaoRespostaDto>> ListarPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            var movimentacao = await _context.MovimentacaoEstoques
                .Where(m =>m.DataMovimentacao >= dataInicio && m.DataMovimentacao <= dataFim)
                .OrderByDescending(m => m.DataMovimentacao)
                .ToListAsync();
            return movimentacao.Select(m => new MovimentacaoRespostaDto
            {
                UsuarioNome = _context.Usuarios.Where(u => u.Id == m.UsuarioId).Select(u => u.NomeUsuario).FirstOrDefault() ?? "Usuário não encontrado",
                ProdutoNome = _context.Produtos.Where(p => p.Id == m.ProdutoId).Select(p => p.NomeProduto).FirstOrDefault() ?? "Produto não encontrado",
                Id = m.Id,
                ProdutoId = m.ProdutoId,
                Quantidade = m.Quantidade,
                TipoMovimentacao = m.TipoMovimentacao,
                DataMovimentacao = m.DataMovimentacao,
                UsuarioId = m.UsuarioId,
                Observacao = m.Observacao,
            });

        }
        public async Task AtualizarEstoqueAsync(int produtoId, int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);
            if (produto == null)
            {
                throw new KeyNotFoundException($"Produto com ID {produtoId} não encontrado.");
            }
            produto.QuantidadeEstoque = quantidade;
            produto.UltimaAtualizacao = DateTime.UtcNow;
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }


    }
}
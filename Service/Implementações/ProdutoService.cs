using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.Produto;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class ProdutoService : IProdutoService
    {
        private readonly ControleEstoqueDbContexto _context;

        public ProdutoService(ControleEstoqueDbContexto context)
        {
            _context = context;
        }

        public async Task<ProdutoRespostaDto> CriarAsync(CriarProdutoDto dto) //Criar Produto
        {
            var fornecedorExistente = await _context.Fornecedores
               .FindAsync(dto.FornecedorId);
            if (fornecedorExistente == null)
            {
                throw new KeyNotFoundException($"Fornecedor com ID {dto.FornecedorId} não encontrado.");
            }

            var categoriaExistente = await _context.Categorias
                .FindAsync(dto.CategoriaId);
            if (categoriaExistente == null)
            {
                throw new KeyNotFoundException($"Categoria com ID {dto.CategoriaId} não encontrada.");
            }

                var produtoExistente = await _context.Produtos
                .Include(f => f.Fornecedor)
                .Include(c => c.Categoria)
                .FirstOrDefaultAsync(p => p.NomeProduto == dto.NomeProduto);

            if (produtoExistente != null)
            {
                throw new InvalidOperationException("Já existe um produto com este nome.");
            }

            var novoProduto = new Produto
            {
                NomeProduto = dto.NomeProduto,
                DescricaoProduto = dto.DescricaoProduto,
                CategoriaId = dto.CategoriaId,
                FornecedorId = dto.FornecedorId,
                PrecoProduto = dto.PrecoProduto,
                QuantidadeEstoque = 0, // Inicializa a quantidade em estoque como 0
                Ativo = true, // Inicializa o produto como ativo
                DataCadastro = DateTime.UtcNow,
                UltimaAtualizacao = DateTime.UtcNow
            };

            _context.Produtos.Add(novoProduto);
            await _context.SaveChangesAsync();

            return new ProdutoRespostaDto
            {
                Id = novoProduto.Id,
                NomeProduto = novoProduto.NomeProduto,
                DescricaoProduto = novoProduto.DescricaoProduto,
                PrecoProduto = novoProduto.PrecoProduto,
                NomeFornecedor = novoProduto.Fornecedor?.NomeFornecedor ??"Fornecedor não encontrado",
                NomeCategoria = novoProduto.Categoria?.NomeCategoria??"Categoria não encontrada",
                QuantidadeEstoque = novoProduto.QuantidadeEstoque,
                Ativo = novoProduto.Ativo,
                DataCadastro = novoProduto.DataCadastro,
                UltimaAtualizacao = novoProduto.UltimaAtualizacao
            };
        }


        public async Task<IEnumerable<ProdutoRespostaDto>> ListarAsync() //Listar Produtos
        {
            var produtos = await _context.Produtos
                .Select(p => new ProdutoRespostaDto
                {
                    Id = p.Id,
                    NomeProduto = p.NomeProduto,
                    DescricaoProduto = p.DescricaoProduto,
                    PrecoProduto = p.PrecoProduto,
                    NomeFornecedor = p.Fornecedor.NomeFornecedor ,
                    NomeCategoria = p.Categoria.NomeCategoria ,
                    QuantidadeEstoque = p.QuantidadeEstoque,
                    Ativo = p.Ativo,
                    DataCadastro = p.DataCadastro,
                    UltimaAtualizacao = p.UltimaAtualizacao
                })
                .ToListAsync();
            return produtos;
        }

        public async Task<ProdutoRespostaDto> ObterPorIdAsync(int id) //Obter Produto por Id
        {
            var produto = await _context.Produtos
                .Include(p => p.Fornecedor)
                .Include(p => p.Categoria)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            if (produto == null)
            {
                throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
            }
            return new ProdutoRespostaDto
            {
                Id = produto.Id,
                NomeProduto = produto.NomeProduto,
                DescricaoProduto = produto.DescricaoProduto,
                PrecoProduto = produto.PrecoProduto,
                NomeFornecedor = produto.Fornecedor.NomeFornecedor,
                NomeCategoria = produto.Categoria.NomeCategoria,
                QuantidadeEstoque = produto.QuantidadeEstoque,
                Ativo = produto.Ativo,
                DataCadastro = produto.DataCadastro,
                UltimaAtualizacao = produto.UltimaAtualizacao
            };
        }

        public async Task<ProdutoRespostaDto> AtualizarAsync(int id, AtualizarProdutoDto dto) //Atualizar Produto
        {
            var fornecedorExistente = await _context.Fornecedores
                .FindAsync(dto.FornecedorId);
            if (fornecedorExistente == null)
            {
                throw new KeyNotFoundException($"Fornecedor com ID {dto.FornecedorId} não encontrado.");
            }

            var categoriaExistente = await _context.Categorias
                .FindAsync(dto.CategoriaId);
            if (categoriaExistente == null)
            {
                throw new KeyNotFoundException($"Categoria com ID {dto.CategoriaId} não encontrada.");
            }

            var produtoExistente = await _context.Produtos
                .Include(p => p.Fornecedor)
                .Include(p => p. Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produtoExistente == null)
            {
                throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
            }
            var produtoAtuaizado = await _context.Produtos
                .FirstOrDefaultAsync(p => p.NomeProduto == dto.NomeProduto && p.Id != id);
            if (produtoAtuaizado != null)
            {
                throw new InvalidOperationException($"Já existe um produto com o nome '{dto.NomeProduto}'.");
            }
            produtoExistente.NomeProduto = dto.NomeProduto;
            produtoExistente.DescricaoProduto = dto.DescricaoProduto;
            produtoExistente.CategoriaId = dto.CategoriaId;
            produtoExistente.FornecedorId = dto.FornecedorId;
            produtoExistente.PrecoProduto = (decimal)dto.PrecoProduto;
            produtoExistente.Ativo = dto.Ativo;
            produtoExistente.UltimaAtualizacao = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new ProdutoRespostaDto
            {
                Id = produtoExistente.Id,
                NomeProduto = produtoExistente.NomeProduto,
                DescricaoProduto = produtoExistente.DescricaoProduto,
                PrecoProduto = produtoExistente.PrecoProduto,
                NomeFornecedor = produtoExistente.Fornecedor.NomeFornecedor,
                NomeCategoria = produtoExistente.Categoria.NomeCategoria,
                QuantidadeEstoque = produtoExistente.QuantidadeEstoque,
                Ativo = produtoExistente.Ativo,
                DataCadastro = produtoExistente.DataCadastro,
                UltimaAtualizacao = produtoExistente.UltimaAtualizacao
            }; 
        }

            public async Task<DeleterRespostaDto> ExcluirAsync(int id) //Deletar Produto
            {
                var produtoExistente = await _context.Produtos.FindAsync(id);
                if (produtoExistente == null)
                {
                    throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
                }
                _context.Produtos.Remove(produtoExistente);
                await _context.SaveChangesAsync();
                return new DeleterRespostaDto
                {
                    mensagem = $"Produto com ID {id} foi excluído com sucesso."
                };
        }
    }
}

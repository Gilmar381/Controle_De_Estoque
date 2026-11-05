using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.Comum;
using Controle_De_estoqueAPI.Dtos.Produto;
using Controle_De_estoqueAPI.Exceptions;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class ProdutoService : IProdutoService
    {
        private readonly ControleEstoqueDbContexto _context;

        public ProdutoService(ControleEstoqueDbContexto context)
        {
            _context = context;
        }

        private static readonly Expression<Func<Produto, ProdutoRespostaDto>> Projecao = p => new ProdutoRespostaDto
        {
            Id = p.Id,
            NomeProduto = p.NomeProduto,
            DescricaoProduto = p.DescricaoProduto,
            PrecoProduto = p.PrecoProduto,
            NomeFornecedor = p.Fornecedor.NomeFornecedor,
            NomeCategoria = p.Categoria.NomeCategoria,
            QuantidadeEstoque = p.QuantidadeEstoque,
            Ativo = p.Ativo,
            DataCadastro = p.DataCadastro,
            UltimaAtualizacao = p.UltimaAtualizacao
        };

        public async Task<ProdutoRespostaDto> CriarAsync(CriarProdutoDto dto)
        {
            if (!await _context.Fornecedores.AnyAsync(f => f.Id == dto.FornecedorId))
                throw new KeyNotFoundException($"Fornecedor com ID {dto.FornecedorId} não encontrado.");

            if (!await _context.Categorias.AnyAsync(c => c.CategoriaID == dto.CategoriaId))
                throw new KeyNotFoundException($"Categoria com ID {dto.CategoriaId} não encontrada.");

            if (await _context.Produtos.AnyAsync(p => p.NomeProduto == dto.NomeProduto))
                throw new ConflitoException("Já existe um produto com este nome.");

            var agora = DateTime.UtcNow;
            var produto = new Produto
            {
                NomeProduto = dto.NomeProduto,
                DescricaoProduto = dto.DescricaoProduto,
                CategoriaId = dto.CategoriaId,
                FornecedorId = dto.FornecedorId,
                PrecoProduto = dto.PrecoProduto,
                QuantidadeEstoque = 0, // o estoque só muda por movimentações
                Ativo = true,
                DataCadastro = agora,
                UltimaAtualizacao = agora
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return await ObterPorIdAsync(produto.Id);
        }

        public async Task<PaginadoDto<ProdutoRespostaDto>> ListarAsync(PaginacaoParametros paginacao, bool incluirInativos = false)
        {
            var query = _context.Produtos.AsNoTracking();

            if (!incluirInativos)
                query = query.Where(p => p.Ativo);

            return await query
                .OrderBy(p => p.NomeProduto).ThenBy(p => p.Id)
                .Select(Projecao)
                .ToPaginadoAsync(paginacao);
        }

        public async Task<ProdutoRespostaDto> ObterPorIdAsync(int id)
        {
            return await _context.Produtos
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(Projecao)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");
        }

        public async Task<ProdutoRespostaDto> AtualizarAsync(int id, AtualizarProdutoDto dto)
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");

            if (dto.NomeProduto != null)
            {
                if (string.IsNullOrWhiteSpace(dto.NomeProduto))
                    throw new RegraDeNegocioException("O nome do produto não pode ser vazio.");

                if (await _context.Produtos.AnyAsync(p => p.NomeProduto == dto.NomeProduto && p.Id != id))
                    throw new ConflitoException($"Já existe um produto com o nome '{dto.NomeProduto}'.");

                produto.NomeProduto = dto.NomeProduto;
            }

            if (dto.DescricaoProduto != null)
                produto.DescricaoProduto = dto.DescricaoProduto;

            if (dto.CategoriaId.HasValue)
            {
                if (!await _context.Categorias.AnyAsync(c => c.CategoriaID == dto.CategoriaId.Value))
                    throw new KeyNotFoundException($"Categoria com ID {dto.CategoriaId} não encontrada.");
                produto.CategoriaId = dto.CategoriaId.Value;
            }

            if (dto.FornecedorId.HasValue)
            {
                if (!await _context.Fornecedores.AnyAsync(f => f.Id == dto.FornecedorId.Value))
                    throw new KeyNotFoundException($"Fornecedor com ID {dto.FornecedorId} não encontrado.");
                produto.FornecedorId = dto.FornecedorId.Value;
            }

            if (dto.PrecoProduto.HasValue)
            {
                if (dto.PrecoProduto.Value <= 0)
                    throw new RegraDeNegocioException("O preço do produto deve ser maior que zero.");
                produto.PrecoProduto = dto.PrecoProduto.Value;
            }

            if (dto.Ativo.HasValue)
                produto.Ativo = dto.Ativo.Value;

            produto.UltimaAtualizacao = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await ObterPorIdAsync(id);
        }

        public async Task<DeleterRespostaDto> ExcluirAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id)
                ?? throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");

            // Exclusão lógica: vendas e movimentações antigas continuam apontando para o produto.
            produto.Ativo = false;
            produto.UltimaAtualizacao = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new DeleterRespostaDto
            {
                mensagem = $"Produto com ID {id} foi desativado com sucesso."
            };
        }
    }
}

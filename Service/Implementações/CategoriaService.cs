using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.Categoria;
using Controle_De_estoqueAPI.Exceptions;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ControleEstoqueDbContexto _context;

        public CategoriaService(ControleEstoqueDbContexto context)
        {
            _context = context;
        }

        private static CategoriaRespostaDto Mapear(Categoria c) => new()
        {
            CategoriaID = c.CategoriaID,
            NomeCategoria = c.NomeCategoria
        };

        public async Task<CategoriaRespostaDto> CriarAsync(CriarCategoriaDto dto)
        {
            if (await _context.Categorias.AnyAsync(c => c.NomeCategoria == dto.NomeCategoria))
                throw new ConflitoException("Categoria já existe.");

            var categoria = new Categoria { NomeCategoria = dto.NomeCategoria };
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return Mapear(categoria);
        }

        public async Task<CategoriaRespostaDto> ObterPorIdAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id)
                ?? throw new KeyNotFoundException("Categoria não encontrada.");
            return Mapear(categoria);
        }

        public async Task<IEnumerable<CategoriaRespostaDto>> ListarAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.NomeCategoria)
                .Select(c => new CategoriaRespostaDto
                {
                    CategoriaID = c.CategoriaID,
                    NomeCategoria = c.NomeCategoria
                })
                .ToListAsync();
        }

        public async Task<CategoriaRespostaDto> AtualizarAsync(int id, AtualizarCategoriaDto dto)
        {
            var categoria = await _context.Categorias.FindAsync(id)
                ?? throw new KeyNotFoundException("Categoria não encontrada.");

            if (await _context.Categorias.AnyAsync(c => c.NomeCategoria == dto.NomeCategoria && c.CategoriaID != id))
                throw new ConflitoException("Já existe uma categoria com este nome.");

            categoria.NomeCategoria = dto.NomeCategoria;
            await _context.SaveChangesAsync();

            return Mapear(categoria);
        }

        public async Task ExcluirAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id)
                ?? throw new KeyNotFoundException("Categoria não encontrada.");

            if (await _context.Produtos.AnyAsync(p => p.CategoriaId == id))
                throw new ConflitoException("Não é possível excluir a categoria, pois existem produtos associados a ela.");

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }
    }
}

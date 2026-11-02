using Controle_De_estoqueAPI.Dtos.Categoria;
using Controle_De_estoqueAPI.Data;
using Microsoft.EntityFrameworkCore;
using Controle_De_estoqueAPI.Service.Interfaces;
using System.Collections.ObjectModel;
using Controle_De_estoqueAPI.Models;

namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ControleEstoqueDbContexto _context;

        public CategoriaService(ControleEstoqueDbContexto context)
        {
            _context = context;
           
        }

        public async Task<CategoriaRespostaDto> CriarAsync(CriarCategoriaDto dto)
        {
            var categoriaExistente = await _context.Categorias.FirstOrDefaultAsync(c => c.NomeCategoria == dto.NomeCategoria);
            if (categoriaExistente != null)
            {
                throw new Exception("Categoria já existe.");
            }

            var categoria = new Categoria
            {
                NomeCategoria = dto.NomeCategoria
               
            };
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return new CategoriaRespostaDto
            {
                CategoriaID = categoria.CategoriaID,
                NomeCategoria = categoria.NomeCategoria,
              
            };
        }

        public async Task<CategoriaRespostaDto> ObterPorIdAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                throw new Exception("Categoria não encontrada.");
            }
            return new CategoriaRespostaDto
            {
                CategoriaID = categoria.CategoriaID,
                NomeCategoria = categoria.NomeCategoria,
              
            };
        }

        public async Task<IEnumerable<CategoriaRespostaDto>> ListarAsync()
        {
            var categorias = await _context.Categorias
                .Select(c => new CategoriaRespostaDto
                {
                    CategoriaID = c.CategoriaID,
                    NomeCategoria = c.NomeCategoria,
                  
                })
                .ToListAsync();
            return categorias;           
        }

        public async Task<CategoriaRespostaDto> AtualizarAsync(int id, AtualizarCategoriaDto dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                throw new Exception("Categoria não encontrada.");
            }
            categoria.NomeCategoria = dto.NomeCategoria;
           
            await _context.SaveChangesAsync();
            return new CategoriaRespostaDto
            {
                CategoriaID = categoria.CategoriaID,
                NomeCategoria = categoria.NomeCategoria,
              
            };
        }

        public async Task ExcluirAsync(int id)
        {
            bool temProduto = await _context.Produtos.AnyAsync(p => p.CategoriaId == id);
            if (temProduto)
            {
                throw new InvalidOperationException("Não é possível excluir a categoria, pois existem produtos associados a ele.");
            }
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                throw new Exception("Categoria não encontrada.");
            }
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }
    }
}

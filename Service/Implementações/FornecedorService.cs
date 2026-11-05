using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.Fornecedor;
using Controle_De_estoqueAPI.Exceptions;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class FornecedorService : IFornecedorService
    {
        private readonly ControleEstoqueDbContexto _context;

        public FornecedorService(ControleEstoqueDbContexto context)
        {
            _context = context;
        }

        private static FornecedorRespostaDto Mapear(Fornecedor f) => new()
        {
            Id = f.Id,
            NomeFornecedor = f.NomeFornecedor,
            CNPJFornecedor = f.CNPJFornecedor,
            TelefoneFornecedor = f.TelefoneFornecedor,
            EmailFornecedor = f.EmailFornecedor,
            EnderecoFornecedor = f.EnderecoFornecedor
        };

        public async Task<FornecedorRespostaDto> CriarAsync(CriarFornecedorDto dto)
        {
            if (await _context.Fornecedores.AnyAsync(f => f.CNPJFornecedor == dto.Cnpj))
                throw new ConflitoException("Já existe um fornecedor com este CNPJ.");

            var fornecedor = new Fornecedor
            {
                NomeFornecedor = dto.Nome,
                CNPJFornecedor = dto.Cnpj,
                TelefoneFornecedor = dto.Telefone,
                EmailFornecedor = dto.Email,
                EnderecoFornecedor = dto.Endereco
            };

            _context.Fornecedores.Add(fornecedor);
            await _context.SaveChangesAsync();
            return Mapear(fornecedor);
        }

        public async Task<FornecedorRespostaDto> ObterPorIdAsync(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id)
                ?? throw new KeyNotFoundException("Fornecedor não encontrado.");
            return Mapear(fornecedor);
        }

        public async Task<IEnumerable<FornecedorRespostaDto>> ListarAsync()
        {
            return await _context.Fornecedores
                .AsNoTracking()
                .OrderBy(f => f.NomeFornecedor)
                .Select(f => new FornecedorRespostaDto
                {
                    Id = f.Id,
                    NomeFornecedor = f.NomeFornecedor,
                    CNPJFornecedor = f.CNPJFornecedor,
                    TelefoneFornecedor = f.TelefoneFornecedor,
                    EmailFornecedor = f.EmailFornecedor,
                    EnderecoFornecedor = f.EnderecoFornecedor
                })
                .ToListAsync();
        }

        public async Task<FornecedorRespostaDto> AtualizarAsync(int id, AtualizarFornecedorDto dto)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id)
                ?? throw new KeyNotFoundException("Fornecedor não encontrado.");

            if (await _context.Fornecedores.AnyAsync(f => f.CNPJFornecedor == dto.Cnpj && f.Id != id))
                throw new ConflitoException("Já existe outro fornecedor com este CNPJ.");

            fornecedor.NomeFornecedor = dto.Nome;
            fornecedor.CNPJFornecedor = dto.Cnpj;
            fornecedor.TelefoneFornecedor = dto.Telefone;
            fornecedor.EmailFornecedor = dto.Email;
            fornecedor.EnderecoFornecedor = dto.Endereco;

            await _context.SaveChangesAsync();
            return Mapear(fornecedor);
        }

        public async Task ExcluirAsync(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id)
                ?? throw new KeyNotFoundException("Fornecedor não encontrado.");

            if (await _context.Produtos.AnyAsync(p => p.FornecedorId == id))
                throw new ConflitoException("Não é possível excluir o fornecedor, pois existem produtos associados a ele.");

            _context.Fornecedores.Remove(fornecedor);
            await _context.SaveChangesAsync();
        }
    }
}
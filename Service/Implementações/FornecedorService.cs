using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.Fornecedor;
using Microsoft.EntityFrameworkCore;
using Controle_De_estoqueAPI.Service.Interfaces;


namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class FornecedorService : IFornecedorService
    {
        private readonly ControleEstoqueDbContexto _context;

        public FornecedorService(ControleEstoqueDbContexto context)
        {
            _context = context;
        }

        public async Task<FornecedorRespostaDto> CriarAsync(CriarFornecedorDto dto)
        {
            var fornecedorExistente = await _context.Fornecedores
                .FirstOrDefaultAsync(c => c.CNPJFornecedor == dto.Cnpj);
            if (fornecedorExistente != null)
            {
                throw new InvalidOperationException("Já existe um fornecedor com este CNPJ.");
            }
            var novoFornecedor = new Fornecedor
            {
                NomeFornecedor = dto.Nome,
                CNPJFornecedor = dto.Cnpj,
                TelefoneFornecedor = dto.Telefone,
                EmailFornecedor = dto.Email,
                EnderecoFornecedor = dto.Endereco
            };
            _context.Fornecedores.Add(novoFornecedor);
            await _context.SaveChangesAsync();
            return new FornecedorRespostaDto
            {
                Id = novoFornecedor.Id,
                NomeFornecedor = novoFornecedor.NomeFornecedor,
                CNPJFornecedor = novoFornecedor.CNPJFornecedor,
                TelefoneFornecedor = novoFornecedor.TelefoneFornecedor,
                EmailFornecedor = novoFornecedor.EmailFornecedor,
                EnderecoFornecedor = novoFornecedor.EnderecoFornecedor
            };
        }

        public async Task<FornecedorRespostaDto> ObterPorIdAsync(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null)
            {
                throw new KeyNotFoundException("Fornecedor não encontrado.");
            }
            return new FornecedorRespostaDto
            {
                Id = fornecedor.Id,
                NomeFornecedor = fornecedor.NomeFornecedor,
                CNPJFornecedor = fornecedor.CNPJFornecedor,
                TelefoneFornecedor = fornecedor.TelefoneFornecedor,
                EmailFornecedor = fornecedor.EmailFornecedor,
                EnderecoFornecedor = fornecedor.EnderecoFornecedor
            };
        }

        public async Task<IEnumerable<FornecedorRespostaDto>> ListarAsync()
        {
            var fornecedores = await _context.Fornecedores
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
             return fornecedores;
        }

        public async Task<FornecedorRespostaDto> AtualizarAsync(int id, AtualizarFornecedorDto dto)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null)
            {
                throw new KeyNotFoundException("Fornecedor não encontrado.");
            }
            fornecedor.NomeFornecedor = dto.Nome;
            fornecedor.CNPJFornecedor = dto.Cnpj;
            fornecedor.TelefoneFornecedor = dto.Telefone;
            fornecedor.EmailFornecedor = dto.Email;
            fornecedor.EnderecoFornecedor = dto.Endereco;
            await _context.SaveChangesAsync();
            return new FornecedorRespostaDto
            {
                Id = fornecedor.Id,
                NomeFornecedor = fornecedor.NomeFornecedor,
                CNPJFornecedor = fornecedor.CNPJFornecedor,
                TelefoneFornecedor = fornecedor.TelefoneFornecedor,
                EmailFornecedor = fornecedor.EmailFornecedor,
                EnderecoFornecedor = fornecedor.EnderecoFornecedor
            };
        }

        public async Task ExcluirAsync(int id)
        {
            bool temProduto = await _context.Produtos.AnyAsync(p => p.FornecedorId == id);
            if (temProduto)
            {
                throw new InvalidOperationException("Não é possível excluir o fornecedor, pois existem produtos associados a ele.");
            }

            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null)
            {
                throw new KeyNotFoundException("Fornecedor não encontrado.");
            }
            _context.Fornecedores.Remove(fornecedor);
            await _context.SaveChangesAsync();
        }
    }   
       
       
    
}

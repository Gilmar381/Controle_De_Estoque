using Controle_De_estoqueAPI.Service.Interfaces;
using Controle_De_estoqueAPI.Data;
using Controle_De_estoqueAPI.Dtos.Perfil;
using Microsoft.EntityFrameworkCore;

namespace Controle_De_estoqueAPI.Service.Implementações
{
    public class PerfilService : IPerfilService
    {
        private readonly ControleEstoqueDbContexto _context;

        public PerfilService(ControleEstoqueDbContexto perfis)
        {
            _context = perfis;
        }
        
        public async Task<IEnumerable<PerfilRespostaDto>> ListarAsync()
        { 
            var perfil = await _context.Perfis
            .Select(p => new PerfilRespostaDto
            {
            Id = p.Id,
            Nome = p.PerfilNome
            })
            .ToListAsync();
            return perfil;
        }

        public async Task<PerfilRespostaDto?> ObterPorIdAsync(int id)
        {
            var perfil = await _context.Perfis.FindAsync(id);
            if (perfil == null)
            {
                return null;
            }
            return new PerfilRespostaDto
            {
             Id = id,
             Nome = perfil.PerfilNome
            };
        }
    }
}

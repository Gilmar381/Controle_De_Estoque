using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Controle_De_estoqueAPI.Dtos.Comum
{
    public class PaginacaoParametros
    {
        [Range(1, 1_000_000)]
        public int Pagina { get; set; } = 1;

        [Range(1, 100)]
        public int TamanhoPagina { get; set; } = 20;
    }

    public class PaginadoDto<T>
    {
        public IReadOnlyList<T> Itens { get; set; } = Array.Empty<T>();
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public int Total { get; set; }
        public int TotalPaginas => TamanhoPagina == 0 ? 0 : (int)Math.Ceiling(Total / (double)TamanhoPagina);
    }

    public static class PaginacaoExtensions
    {
        /// <summary>A query recebida deve estar ordenada, para a paginação ser estável.</summary>
        public static async Task<PaginadoDto<T>> ToPaginadoAsync<T>(this IQueryable<T> query, PaginacaoParametros p)
        {
            var total = await query.CountAsync();
            var itens = await query
                .Skip((p.Pagina - 1) * p.TamanhoPagina)
                .Take(p.TamanhoPagina)
                .ToListAsync();

            return new PaginadoDto<T>
            {
                Itens = itens,
                Pagina = p.Pagina,
                TamanhoPagina = p.TamanhoPagina,
                Total = total
            };
        }
    }
}

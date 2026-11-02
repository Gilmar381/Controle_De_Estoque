using Controle_De_estoqueAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.MovimentacaoEstoque
{
    public class CriarMovimentacaoDto
    {
        [Required]
        public int ProdutoId { get; set; }

        [Required]          
        public int UsuarioId { get; set; }

        [Required]
        public int FornecedorId { get; set; } 

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

 

        [Required]
        [MaxLength(500)]
        public  string Observacao { get; set; }
    }
}
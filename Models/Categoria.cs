using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Controle_De_estoqueAPI.Models
{
    public class Categoria
    {
        [Key]
        public int CategoriaID { get; set; }
        [Required]
        [StringLength(100)]
        public string NomeCategoria { get; set; }

        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
               
    }
}

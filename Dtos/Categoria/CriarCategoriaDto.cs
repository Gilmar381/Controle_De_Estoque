using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Categoria
{
    public class CriarCategoriaDto
    {
        [Required]
        [MaxLength(100)]
        public string NomeCategoria { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Controle_De_estoqueAPI.Models
{
    public class Perfil
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string PerfilNome { get; set; }
        public  ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}

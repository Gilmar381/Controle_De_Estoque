using System.ComponentModel.DataAnnotations;

namespace Controle_De_estoqueAPI.Dtos.Usuario
{
    /// <summary>Atualização parcial: só os campos enviados (não nulos) são alterados.</summary>
    public class AtualizarUsuarioDto
    {
        [MaxLength(100)]
        public string? Nome { get; set; }

        [EmailAddress]
        [MaxLength(256)]
        public string? Email { get; set; }

        [Range(1, int.MaxValue)]
        public int? PerfilId { get; set; }

        public bool? Ativo { get; set; }
    }
}

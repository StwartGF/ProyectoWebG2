using System.ComponentModel.DataAnnotations;

namespace ProyectoWebG2Api.Models
{
    public class CrearInstructorDto
    {
        [Required]
        [StringLength(20)]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string ContrasenaHash { get; set; } = string.Empty;
    }
}
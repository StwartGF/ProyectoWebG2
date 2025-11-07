// Models/PerfilRequestModel.cs
using System.ComponentModel.DataAnnotations;
namespace SM_ProyectoAPI.Models
{
    public class PerfilRequestModel
    {
        [Required] public int ConsecutivoUsuario { get; set; }
        [Required] public string Identificacion { get; set; } = "";
        [Required] public string Nombre { get; set; } = "";
        [Required] public string Apellidos { get; set; } = "";
        [Required, EmailAddress] public string CorreoElectronico { get; set; } = "";
        public string? Telefono { get; set; }
        public DateTime? Fecha_Nacimiento { get; set; }
        [Required] public string NombreUsuario { get; set; } = "";
    }
}
// Models/RegistroUsuarioRequestModel.cs
using System.ComponentModel.DataAnnotations;
namespace SM_ProyectoAPI.Models
{
    public class RegistroUsuarioRequestModel
    {
        [Required] public string Identificacion { get; set; } = "";
        [Required] public string Nombre { get; set; } = "";
        [Required] public string Apellidos { get; set; } = "";
        [Required, EmailAddress] public string CorreoElectronico { get; set; } = "";
        public string? Telefono { get; set; }
        public DateTime? Fecha_Nacimiento { get; set; }
        [Required] public string NombreUsuario { get; set; } = "";
        [Required] public string Contrasenna { get; set; } = "";
        [Required] public string ContrasennaConfirmar { get; set; } = "";
        public int? IdRol { get; set; } = 2;
    }
}
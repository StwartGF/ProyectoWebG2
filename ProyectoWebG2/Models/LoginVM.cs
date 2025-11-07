using System.ComponentModel.DataAnnotations;

namespace ProyectoWebG2.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Contrasena { get; set; } = string.Empty;
    }
}

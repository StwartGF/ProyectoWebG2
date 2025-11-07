using System.ComponentModel.DataAnnotations;

namespace ProyectoWebG2.Models
{
    // Este modelo se usa en las Vistas (Login, Registro, Recuperar)
    // y se serializa cuando llamamos a la API.
    public class UsuarioModel
    {
        // --------- Comunes en login / sesión ----------
        public int IdUsuario { get; set; }

        [Display(Name = "Usuario o correo")]
        public string UsuarioOCorreo => string.IsNullOrWhiteSpace(NombreUsuario) ? CorreoElectronico : NombreUsuario;

        // --------- Autenticación / sesión ----------
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Contrasena { get; set; } = string.Empty;

        // --------- Campos para Registro (ampliado) ----------
        [Required(ErrorMessage = "La identificación es requerida")]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son requeridos")]
        public string Apellidos { get; set; } = string.Empty;

        [Phone]
        public string Telefono { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime? Fecha_Nacimiento { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar contraseña")]
        public string ContrasenaConfirmar { get; set; } = string.Empty;

        // --------- Datos de perfil devueltos por la API ----------
        public string NombrePerfil { get; set; } = string.Empty;
    }
}

// Models/ValidarSesionResponse.cs
namespace SM_ProyectoAPI.Models
{
    public class ValidarSesionResponse
    {
        public int ConsecutivoUsuario { get; set; }      // IdUsuario
        public string Identificacion { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public string CorreoElectronico { get; set; } = "";
        public string NombreUsuario { get; set; } = "";
        public bool Estado { get; set; } = true;
        public int ConsecutivoPerfil { get; set; }       // IdRol
        public string NombrePerfil { get; set; } = "";
        public string ContrasenaHash { get; set; } = ""; // para validar en API, no enviar a MVC
    }
}
namespace ProyectoWebG2.Models;

public class RegisterModel
{
    public string Identificacion { get; set; } = "0001";
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public string ContrasenaConfirmar { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public DateTime? FechaNacimiento { get; set; }
}

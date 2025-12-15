namespace ProyectoWebG2.Models
{
    public class InstructorVM
    {
        public int Id { get; set; }

        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;

        public string ContrasenaHash { get; set; } = string.Empty;

        public string NombreCompleto => $"{Nombre} {Apellidos}";
    }
}




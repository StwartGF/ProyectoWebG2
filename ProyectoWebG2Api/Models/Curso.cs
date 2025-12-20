namespace ProyectoWebG2Api.Models
{
    public class Curso
    {
        public int IdCurso { get; set; }
        public string NombreCurso { get; set; } = string.Empty;
        public DateTime FechaDeInicio { get; set; }
        public DateTime FechaDeFinalizacion { get; set; }
        public int DuracionCurso { get; set; }
        public int CuposDisponibles { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Modalidad { get; set; } = string.Empty;

        public int? IdInstructor { get; set; }   // FK a Usuario.IdUsuario (rol instructor)
        public string NombreInstructor { get; set; } = string.Empty; // solo para lectura
    }
}

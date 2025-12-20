namespace ProyectoWebG2Api.Models
{
    public class CursoInstructorDto
    {
        public int IdCurso { get; set; }
        public string NombreCurso { get; set; } = string.Empty;
        public DateTime FechaDeInicio { get; set; }
        public DateTime FechaDeFinalizacion { get; set; }
        public int DuracionCurso { get; set; }
        public int CuposDisponibles { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Modalidad { get; set; } = string.Empty;
    }

    public class EstudianteCursoDto
    {
        public int IdUsuario { get; set; }
        public int? IdMatricula { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
    }

    public class AsistenciaItemDto
    {
        public int IdUsuario { get; set; }
        public string EstadoAsistencia { get; set; } = string.Empty;
    }

    public class AsistenciaRequestDto
    {
        public DateTime Fecha { get; set; }
        public List<AsistenciaItemDto> Items { get; set; } = new();
    }

    public class CalificacionItemDto
    {
        public int IdUsuario { get; set; }
        public decimal Calificacion { get; set; }
    }

    public class CalificacionesRequestDto
    {
        public List<CalificacionItemDto> Items { get; set; } = new();
    }
}

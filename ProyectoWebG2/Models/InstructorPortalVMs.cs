using System.ComponentModel.DataAnnotations;

namespace ProyectoWebG2.Models
{
    public class CursoInstructorVM
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

    public class EstudianteCursoVM
    {
        public int IdMatricula { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
    }

    public class AsistenciaItemVM
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        public string EstadoAsistencia { get; set; } = "Presente";
    }

    public class AsistenciaFormVM
    {
        public int CursoId { get; set; }
        public DateTime Fecha { get; set; }
        public List<AsistenciaItemVM> Items { get; set; } = new();
    }

    public class CalificacionItemVM
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public double? Calificacion { get; set; }
    }

    public class CalificacionesFormVM
    {
        public int CursoId { get; set; }
        public List<CalificacionItemVM> Items { get; set; } = new();
    }
}

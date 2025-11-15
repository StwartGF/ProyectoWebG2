namespace ProyectoWebG2.Models
{
    public class AsistenciaModel
    {
        public int IdAsistencia { get; set; }
        public int IdUsuario { get; set; }
        public int IdCurso { get; set; }
        public DateTime Fecha { get; set; }
        public string EstadoAsistencia { get; set; } = string.Empty;
    }
}

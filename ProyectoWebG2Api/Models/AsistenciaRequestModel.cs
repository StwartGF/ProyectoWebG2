namespace ProyectoWebG2Api.Models
{
    public class AsistenciaRequestModel
    {
        public int IdAsistencia { get; set; }   // para editar/eliminar
        public int IdUsuario { get; set; }
        public int IdCurso { get; set; }
        public DateTime Fecha { get; set; }
        public string EstadoAsistencia { get; set; } = string.Empty;
    }
}

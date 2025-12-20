using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoWebG2.Models
{
    public class CursoVM
    {
        public int IdCurso { get; set; }

        [Required]
        [Display(Name = "Nombre del curso")]
        public string NombreCurso { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de inicio")]
        public DateTime FechaDeInicio { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de finalización")]
        public DateTime FechaDeFinalizacion { get; set; }

        [Required]
        [Display(Name = "Duración (horas)")]
        public int DuracionCurso { get; set; }

        [Required]
        [Display(Name = "Cupos disponibles")]
        public int CuposDisponibles { get; set; }

        [Required]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Modalidad")]
        public string Modalidad { get; set; } = string.Empty;

        [Display(Name = "Instructor")]
        public int? IdInstructor { get; set; }

        // Opcional: nombre del instructor para mostrar en la vista
        public string NombreInstructor { get; set; } = string.Empty;
    }
}

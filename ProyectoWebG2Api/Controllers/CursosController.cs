using Microsoft.AspNetCore.Mvc;
using ProyectoWebG2Api.Models;

namespace ProyectoWebG2Api.Controllers
{

        [ApiController]
        [Route("api/[controller]")]
        public class CursosController : ControllerBase
        {
            [HttpGet]
            public IActionResult GetCursos()
            {
                var cursos = new List<Curso>
            {
                new() { Id = 1, Nombre = "Programación Avanzada en Web", Duracion = "16 semanas", Descripcion = "Aprende sobre proyectos web." },
                new() { Id = 2, Nombre = "Algebra Lineal", Duracion = "8 semanas", Descripcion = "Aprende las bases de la Ingeniería" },
                new() { Id = 3, Nombre = "Bases de datos multidimensionales", Duracion = "10 semanas", Descripcion = "Aprende sobre bases de datos" }
            };

                return Ok(cursos);
            }
        }
}

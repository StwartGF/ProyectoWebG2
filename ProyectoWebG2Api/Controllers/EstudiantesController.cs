using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProyectoWebG2Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstudiantesController : ControllerBase
    {
        private readonly IConfiguration _cfg;

        public EstudiantesController(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        [HttpGet("CursosDisponibles")]
        public IActionResult GetCursosDisponibles()
        {
            try
            {
                using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);
                var cursos = cn.Query(
                    "ObtenerCursosDisponibles",
                    commandType: CommandType.StoredProcedure
                );

                return Ok(cursos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("HorariosCurso/{idCurso}")]
        public IActionResult GetHorariosCurso(int idCurso)
        {
            try
            {
                using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);

                var p = new DynamicParameters();
                p.Add("@IdCurso", idCurso);

                var horarios = cn.Query(
                    "ObtenerHorariosCurso",
                    p,
                    commandType: CommandType.StoredProcedure
                );

                return Ok(horarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("Matricular")]
        public IActionResult MatricularEstudiante([FromBody] MatricularRequest request)
        {
            try
            {
                using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);

                var p = new DynamicParameters();
                p.Add("@IdUsuario", request.IdUsuario);
                p.Add("@IdCurso", request.IdCurso);
                p.Add("@IdHorario", request.IdHorario);

                var resultado = cn.QueryFirstOrDefault<int>(
                    "MatricularEstudiante",
                    p,
                    commandType: CommandType.StoredProcedure
                );

                if (resultado == -1)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Ya estás matriculado en este curso."
                    });
                }

                if (resultado == 0)
                {
                    return Ok(new
                    {
                        success = false,
                        message = "No se pudo realizar la matrícula."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "¡Matrícula realizada con éxito!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error en el servidor: " + ex.Message
                });
            }
        }


        [HttpGet("CursosMatriculados/{idUsuario}")]
        public IActionResult GetCursosMatriculados(int idUsuario)
        {
            try
            {
                using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);

                var p = new DynamicParameters();
                p.Add("@IdUsuario", idUsuario);

                var cursos = cn.Query(
                    "ObtenerCursosMatriculados",
                    p,
                    commandType: CommandType.StoredProcedure
                );

                return Ok(cursos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("Desmatricular/{idUsuario}/{idCurso}")]
        public IActionResult DesmatricularEstudiante(int idUsuario, int idCurso)
        {
            try
            {
                using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);

                var p = new DynamicParameters();
                p.Add("@IdUsuario", idUsuario);
                p.Add("@IdCurso", idCurso);

                var resultado = cn.QueryFirstOrDefault<int>(
                    "DesmatricularEstudiante",
                    p,
                    commandType: CommandType.StoredProcedure
                );

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpGet("Calificaciones/{idUsuario}")]
        public IActionResult GetCalificaciones(int idUsuario)
        {
            try
            {
                using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);

                var p = new DynamicParameters();
                p.Add("@IdUsuario", idUsuario);

                var calificaciones = cn.Query(
                    "ObtenerCalificacionesPorUsuario",
                    p,
                    commandType: CommandType.StoredProcedure
                );

                return Ok(calificaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }


    public class MatricularRequest
    {
        public int IdUsuario { get; set; }
        public int IdCurso { get; set; }
        public int? IdHorario { get; set; }
    }
}

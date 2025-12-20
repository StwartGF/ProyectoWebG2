using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProyectoWebG2Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalificacionesController : ControllerBase
    {
        private readonly IConfiguration _cfg;

        public CalificacionesController(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        [HttpGet]
        public IActionResult GetCalificaciones()
        {
            try
            {
                using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);
                var calificaciones = cn.Query(
                    "ObtenerTodasLasCalificaciones",
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
}

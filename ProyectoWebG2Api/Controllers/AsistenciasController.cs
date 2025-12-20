using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProyectoWebG2Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsistenciasController : ControllerBase
    {
        private readonly IConfiguration _cfg;

        public AsistenciasController(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        [HttpGet]
        public IActionResult GetAsistencias()
        {
            try
            {
                using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);
                var asistencias = cn.Query(
                    "ObtenerTodasLasAsistencias",
                    commandType: CommandType.StoredProcedure
                );
                return Ok(asistencias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

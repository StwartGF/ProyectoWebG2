using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProyectoWebG2Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatriculasController : ControllerBase
    {
        private readonly IConfiguration _cfg;

        public MatriculasController(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        [HttpGet]
        public IActionResult GetMatriculas()
        {
            try
            {
                using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);
                var matriculas = cn.Query(
                    "ObtenerTodasLasMatriculas",
                    commandType: CommandType.StoredProcedure
                );
                return Ok(matriculas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

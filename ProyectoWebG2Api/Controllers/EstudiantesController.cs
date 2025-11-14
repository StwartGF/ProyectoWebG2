using Microsoft.AspNetCore.Http;
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

        [HttpGet("Calificaciones/{idUsuario}")]
        public IActionResult GetCalificaciones(int idUsuario)
        {
            using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);
            var p = new DynamicParameters();
            p.Add("@IdUsuario", idUsuario);
            var result = cn.Query("ObtenerCalificacionesPorUsuario", p, commandType: CommandType.StoredProcedure);
            return Ok(result);
        }
    }
}

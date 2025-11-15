using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoWebG2Api.Models;
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

        private SqlConnection GetConnection()
            => new SqlConnection(_cfg.GetConnectionString("BDConnection"));

        // GET: api/Asistencias
        [HttpGet]
        public IActionResult Get()
        {
            using var cn = GetConnection();
            var result = cn.Query("spAsistencia_Listar",
                                  commandType: CommandType.StoredProcedure);
            return Ok(result);
        }

        // GET: api/Asistencias/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using var cn = GetConnection();
            var p = new DynamicParameters();
            p.Add("@IdAsistencia", id);

            var result = cn.QueryFirstOrDefault("spAsistencia_ObtenerPorId",
                                                p,
                                                commandType: CommandType.StoredProcedure);

            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/Asistencias
        [HttpPost]
        public IActionResult Post([FromBody] AsistenciaRequestModel modelo)
        {
            using var cn = GetConnection();
            var p = new DynamicParameters();
            p.Add("@IdUsuario", modelo.IdUsuario);
            p.Add("@IdCurso", modelo.IdCurso);
            p.Add("@Fecha", modelo.Fecha);
            p.Add("@EstadoAsistencia", modelo.EstadoAsistencia);

            var nuevoId = cn.ExecuteScalar<int>("spAsistencia_Insertar",
                                                p,
                                                commandType: CommandType.StoredProcedure);

            modelo.IdAsistencia = nuevoId;
            return Ok(modelo);
        }

        // PUT: api/Asistencias/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] AsistenciaRequestModel modelo)
        {
            if (id != modelo.IdAsistencia) return BadRequest();

            using var cn = GetConnection();
            var p = new DynamicParameters();
            p.Add("@IdAsistencia", modelo.IdAsistencia);
            p.Add("@IdUsuario", modelo.IdUsuario);
            p.Add("@IdCurso", modelo.IdCurso);
            p.Add("@Fecha", modelo.Fecha);
            p.Add("@EstadoAsistencia", modelo.EstadoAsistencia);

            cn.Execute("spAsistencia_Actualizar",
                       p,
                       commandType: CommandType.StoredProcedure);

            return NoContent();
        }

        // DELETE: api/Asistencias/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using var cn = GetConnection();
            var p = new DynamicParameters();
            p.Add("@IdAsistencia", id);

            cn.Execute("spAsistencia_Eliminar",
                       p,
                       commandType: CommandType.StoredProcedure);

            return NoContent();
        }
    }
}

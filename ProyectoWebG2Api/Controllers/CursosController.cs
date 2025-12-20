using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoWebG2Api.Models;
using System.Data;

namespace ProyectoWebG2Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CursosController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        private readonly string _connectionString;

        public CursosController(IConfiguration cfg)
        {
            _cfg = cfg;
            _connectionString = _cfg.GetConnectionString("BDConnection")
                ?? throw new InvalidOperationException(
                    "Falta la cadena de conexión 'BDConnection' en appsettings.json del ProyectoWebG2Api.");
        }

        // ==========================================
        // GET: api/cursos
        // Lista todos los cursos
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetCursos()
        {
            var lista = new List<Curso>();

            await using var cn = new SqlConnection(_connectionString);
            await cn.OpenAsync();

            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"
                SELECT 
                    c.IdCurso,
                    c.NombreCurso,
                    c.FechaDeInicio,
                    c.FechaDeFinalizacion,
                    c.DuracionCurso,
                    c.CuposDisponibles,
                    c.Categoria,
                    c.Modalidad,
                    c.IdInstructor,
                    NombreInstructor = 
                        COALESCE(u.Nombre + ' ' + u.Apellidos, 'Por asignar')
                FROM dbo.Curso AS c
                LEFT JOIN dbo.Usuario AS u 
                    ON u.IdUsuario = c.IdInstructor;";

            using var rd = await cmd.ExecuteReaderAsync();

            while (await rd.ReadAsync())
            {
                lista.Add(MapCurso(rd));
            }

            return Ok(lista);
        }

        // ==========================================
        // GET: api/cursos/{id}
        // Un curso específico (para editar)
        // ==========================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCurso(int id)
        {
            await using var cn = new SqlConnection(_connectionString);
            await cn.OpenAsync();

            using var cmd = cn.CreateCommand();
            cmd.CommandText = @"
                SELECT 
                    c.IdCurso,
                    c.NombreCurso,
                    c.FechaDeInicio,
                    c.FechaDeFinalizacion,
                    c.DuracionCurso,
                    c.CuposDisponibles,
                    c.Categoria,
                    c.Modalidad,
                    c.IdInstructor,
                    NombreInstructor = 
                        COALESCE(u.Nombre + ' ' + u.Apellidos, 'Por asignar')
                FROM dbo.Curso AS c
                LEFT JOIN dbo.Usuario AS u 
                    ON u.IdUsuario = c.IdInstructor
                WHERE c.IdCurso = @Id;";

            cmd.Parameters.AddWithValue("@Id", id);

            using var rd = await cmd.ExecuteReaderAsync();

            if (!await rd.ReadAsync())
                return NotFound("Curso no encontrado.");

            var curso = MapCurso(rd);
            return Ok(curso);
        }

        // ==========================================
        // POST: api/cursos
        // Crear curso
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> CrearCurso([FromBody] Curso modelo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await using var cn = new SqlConnection(_connectionString);
            await cn.OpenAsync();

            using var cmd = cn.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = @"
                INSERT INTO dbo.Curso
                    (NombreCurso,
                     FechaDeInicio,
                     FechaDeFinalizacion,
                     DuracionCurso,
                     CuposDisponibles,
                     Categoria,
                     Modalidad,
                     IdInstructor)
                VALUES
                    (@NombreCurso,
                     @FechaDeInicio,
                     @FechaDeFinalizacion,
                     @DuracionCurso,
                     @CuposDisponibles,
                     @Categoria,
                     @Modalidad,
                     @IdInstructor);

                SELECT SCOPE_IDENTITY();";

            cmd.Parameters.AddWithValue("@NombreCurso", modelo.NombreCurso);
            cmd.Parameters.AddWithValue("@FechaDeInicio", modelo.FechaDeInicio);
            cmd.Parameters.AddWithValue("@FechaDeFinalizacion", modelo.FechaDeFinalizacion);
            cmd.Parameters.AddWithValue("@DuracionCurso", modelo.DuracionCurso);
            cmd.Parameters.AddWithValue("@CuposDisponibles", modelo.CuposDisponibles);
            cmd.Parameters.AddWithValue("@Categoria", modelo.Categoria);
            cmd.Parameters.AddWithValue("@Modalidad", modelo.Modalidad);
            cmd.Parameters.AddWithValue("@IdInstructor",
                (object?)modelo.IdInstructor ?? DBNull.Value);

            var idObj = await cmd.ExecuteScalarAsync();
            var id = Convert.ToInt32(idObj);

            modelo.IdCurso = id;

            return CreatedAtAction(nameof(GetCurso), new { id }, modelo);
        }

        // ==========================================
        // PUT: api/cursos/{id}
        // Actualizar curso (incluye asignar instructor)
        // ==========================================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCurso(int id, [FromBody] Curso modelo)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await using var cn = new SqlConnection(_connectionString);
            await cn.OpenAsync();

            using var cmd = cn.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = @"
                UPDATE dbo.Curso
                SET
                    NombreCurso        = @NombreCurso,
                    FechaDeInicio      = @FechaDeInicio,
                    FechaDeFinalizacion= @FechaDeFinalizacion,
                    DuracionCurso      = @DuracionCurso,
                    CuposDisponibles   = @CuposDisponibles,
                    Categoria          = @Categoria,
                    Modalidad          = @Modalidad,
                    IdInstructor       = @IdInstructor
                WHERE IdCurso = @Id;";

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@NombreCurso", modelo.NombreCurso);
            cmd.Parameters.AddWithValue("@FechaDeInicio", modelo.FechaDeInicio);
            cmd.Parameters.AddWithValue("@FechaDeFinalizacion", modelo.FechaDeFinalizacion);
            cmd.Parameters.AddWithValue("@DuracionCurso", modelo.DuracionCurso);
            cmd.Parameters.AddWithValue("@CuposDisponibles", modelo.CuposDisponibles);
            cmd.Parameters.AddWithValue("@Categoria", modelo.Categoria);
            cmd.Parameters.AddWithValue("@Modalidad", modelo.Modalidad);
            cmd.Parameters.AddWithValue("@IdInstructor",
                (object?)modelo.IdInstructor ?? DBNull.Value);

            var filas = await cmd.ExecuteNonQueryAsync();

            if (filas == 0)
                return NotFound("Curso no encontrado.");

            return NoContent(); // 204
        }

        // ==========================================
        // Mapeo de SqlDataReader -> Curso
        // ==========================================
        private static Curso MapCurso(SqlDataReader rd)
        {
            return new Curso
            {
                IdCurso = rd.GetInt32(rd.GetOrdinal("IdCurso")),
                NombreCurso = rd.GetString(rd.GetOrdinal("NombreCurso")),
                FechaDeInicio = rd.GetDateTime(rd.GetOrdinal("FechaDeInicio")),
                FechaDeFinalizacion = rd.GetDateTime(rd.GetOrdinal("FechaDeFinalizacion")),
                DuracionCurso = rd.GetInt32(rd.GetOrdinal("DuracionCurso")),
                CuposDisponibles = rd.GetInt32(rd.GetOrdinal("CuposDisponibles")),
                Categoria = rd.GetString(rd.GetOrdinal("Categoria")),
                Modalidad = rd.GetString(rd.GetOrdinal("Modalidad")),
                IdInstructor = rd.IsDBNull(rd.GetOrdinal("IdInstructor"))
                    ? (int?)null
                    : rd.GetInt32(rd.GetOrdinal("IdInstructor")),
                NombreInstructor = rd.GetString(rd.GetOrdinal("NombreInstructor"))
            };
        }
    }
}



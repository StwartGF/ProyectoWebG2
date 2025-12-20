using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoWebG2Api.Models;
using System.Data;

namespace ProyectoWebG2Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        private readonly string _connectionString;

        public AdminController(IConfiguration cfg)
        {
            _cfg = cfg;

            // ESTE NOMBRE DEBE COINCIDIR CON appsettings.json DEL API
            //  "ConnectionStrings": { "BDConnection": "..." }
            _connectionString = _cfg.GetConnectionString("BDConnection")
                ?? throw new InvalidOperationException(
                    "Falta la cadena de conexión 'BDConnection' en appsettings.json del ProyectoWebG2Api.");
        }

        // ================= MATRICULAS =================
        [HttpGet("matriculas")]
        public IActionResult GetMatriculas()
        {
            using var cn = new SqlConnection(_connectionString);
            var result = cn.Query("ObtenerTodasLasMatriculas",
                                  commandType: CommandType.StoredProcedure);
            return Ok(result);
        }

        // ================= ASIGNAR CURSO A INSTRUCTOR =================
        [HttpPost("asignar-curso")]
        public IActionResult AsignarCurso([FromBody] AsignarCursoRequest req)
        {
            using var cn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@IdCurso", req.IdCurso);
            p.Add("@IdInstructor", req.IdInstructor);

            cn.Execute("AsignarCursoAInstructor",
                       p,
                       commandType: CommandType.StoredProcedure);

            return Ok();
        }

        public class AsignarCursoRequest
        {
            public int IdCurso { get; set; }
            public int IdInstructor { get; set; }
        }

        // ================= CALIFICACIONES =================
        [HttpGet("calificaciones")]
        public IActionResult GetCalificaciones()
        {
            using var cn = new SqlConnection(_connectionString);
            var result = cn.Query("ObtenerTodasLasCalificaciones",
                                  commandType: CommandType.StoredProcedure);
            return Ok(result);
        }

        // ================= ASISTENCIAS =================
        [HttpGet("asistencias")]
        public IActionResult GetAsistencias()
        {
            using var cn = new SqlConnection(_connectionString);
            var result = cn.Query("ObtenerTodasLasAsistencias",
                                  commandType: CommandType.StoredProcedure);
            return Ok(result);
        }

        // =====================================================
        // ================ INSTRUCTORES =======================
        // =====================================================

        // ----- CREAR INSTRUCTOR (IdRol = 3) -----------------
        [HttpPost("instructores")]
        public IActionResult CrearInstructor([FromBody] CrearInstructorRequest req)
        {
            using var cn = new SqlConnection(_connectionString);

            var p = new DynamicParameters();
            p.Add("@Cedula", req.Cedula);
            p.Add("@Nombre", req.Nombre);
            p.Add("@Apellidos", req.Apellidos);
            p.Add("@Telefono", req.Telefono);
            p.Add("@Correo", req.Correo);
            p.Add("@ContrasenaHash", req.ContrasenaHash);
            p.Add("@IdRol", 3); // 3 = Instructor

            // Usa el SP general de registro de usuarios
            var id = cn.ExecuteScalar<int>("Registro",
                                           p,
                                           commandType: CommandType.StoredProcedure);

            return Ok(id);
        }

        // ----- LISTAR SOLO INSTRUCTORES ---------------------
        // GET: api/admin/instructores
        [HttpGet("instructores")]
        public IActionResult GetInstructores()
        {
            using var cn = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT 
                    IdUsuario      AS Id,
                    Cedula,
                    Nombre,
                    Apellidos,
                    Telefono,
                    Correo
                FROM dbo.Usuario
                WHERE IdRol = 3;";           // solo instructores

            var lista = cn.Query<InstructorDto>(sql);
            return Ok(lista);
        }

        // ----- OBTENER UN INSTRUCTOR POR ID -----------------
        // GET: api/admin/instructores/8
        [HttpGet("instructores/{id:int}")]
        public IActionResult GetInstructorPorId(int id)
        {
            using var cn = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT 
                    IdUsuario      AS Id,
                    Cedula,
                    Nombre,
                    Apellidos,
                    Telefono,
                    Correo
                FROM dbo.Usuario
                WHERE IdUsuario = @Id AND IdRol = 3;";

            var inst = cn.QueryFirstOrDefault<InstructorDto>(sql, new { Id = id });

            if (inst is null)
                return NotFound("Instructor no encontrado.");

            return Ok(inst);
        }

        // ----- EDITAR DATOS DEL INSTRUCTOR ------------------
        // PUT: api/admin/instructores/8
        [HttpPut("instructores/{id:int}")]
        public IActionResult ActualizarInstructor(int id, [FromBody] ActualizarInstructorRequest req)
        {
            using var cn = new SqlConnection(_connectionString);

            const string sql = @"
                UPDATE dbo.Usuario
                SET 
                    Cedula   = @Cedula,
                    Nombre   = @Nombre,
                    Apellidos= @Apellidos,
                    Telefono = @Telefono,
                    Correo   = @Correo
                WHERE IdUsuario = @Id AND IdRol = 3;";

            var filas = cn.Execute(sql, new
            {
                Id = id,
                req.Cedula,
                req.Nombre,
                req.Apellidos,
                req.Telefono,
                req.Correo
            });

            if (filas == 0)
                return NotFound("Instructor no encontrado o no es de rol 3.");

            return NoContent(); // 204 OK sin cuerpo
        }
    }

    // ====== DTOs / modelos para la parte de instructores ======

    public class CrearInstructorRequest
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string ContrasenaHash { get; set; } = string.Empty;
    }

    // Lo usamos para listar y para detalle
    public class InstructorDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
    }

    public class ActualizarInstructorRequest
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
    }
}


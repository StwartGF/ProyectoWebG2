using System.Data;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoWebG2Api.Models;

namespace ProyectoWebG2Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstructoresController : ControllerBase
    {
        private readonly string _connectionString;

        public InstructoresController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("BDConnection")
                ?? throw new InvalidOperationException(
                    "Falta la cadena de conexión 'BDConnection' en appsettings.json del ProyectoWebG2Api.");
        }

        // GET: api/instructores/{instructorId}/cursos
        [HttpGet("{instructorId:int}/cursos")]
        public async Task<IActionResult> GetCursosPorInstructor(int instructorId)
        {
            await using var cn = new SqlConnection(_connectionString);
            const string sql = @"
                SELECT IdCurso,
                       NombreCurso,
                       FechaDeInicio,
                       FechaDeFinalizacion,
                       DuracionCurso,
                       CuposDisponibles,
                       Categoria,
                       Modalidad
                FROM dbo.Curso
                WHERE IdInstructor = @InstructorId;";

            var cursos = await cn.QueryAsync<CursoInstructorDto>(sql, new { InstructorId = instructorId });
            return Ok(cursos);
        }

        // GET: api/instructores/{instructorId}/cursos/{cursoId}/estudiantes
        [HttpGet("{instructorId:int}/cursos/{cursoId:int}/estudiantes")]
        public async Task<IActionResult> GetEstudiantesDelCurso(int instructorId, int cursoId)
        {
            await using var cn = new SqlConnection(_connectionString);
            await cn.OpenAsync();

            var pertenece = await CursoPerteneceAInstructor(cn, cursoId, instructorId);
            if (!pertenece)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "El curso no pertenece al instructor.");
            }

            const string sql = @"
                SELECT m.IdMatricula,
                       u.IdUsuario,
                       u.Nombre,
                       u.Apellidos,
                       u.Correo
                FROM dbo.Matricula AS m
                INNER JOIN dbo.Usuario AS u
                    ON u.IdUsuario = m.IdUsuario
                WHERE m.IdCurso = @CursoId
                  AND m.Estado = 'Activa';";

            var estudiantes = await cn.QueryAsync<EstudianteCursoDto>(sql, new { CursoId = cursoId });
            return Ok(estudiantes);
        }

        // POST: api/instructores/{instructorId}/cursos/{cursoId}/asistencia
        [HttpPost("{instructorId:int}/cursos/{cursoId:int}/asistencia")]
        public async Task<IActionResult> RegistrarAsistencia(
            int instructorId,
            int cursoId,
            [FromBody] AsistenciaRequestDto request)
        {
            if (request is null || request.Items.Count == 0)
                return BadRequest("No se enviaron asistencias.");

            await using var cn = new SqlConnection(_connectionString);
            await cn.OpenAsync();

            var pertenece = await CursoPerteneceAInstructor(cn, cursoId, instructorId);
            if (!pertenece)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "El curso no pertenece al instructor.");
            }

            using var tx = cn.BeginTransaction();

            try
            {
                foreach (var item in request.Items)
                {
                    // Validar que el estudiante esté matriculado en el curso
                    const string matriculaSql = @"
                        SELECT COUNT(1)
                        FROM dbo.Matricula
                        WHERE IdCurso = @CursoId
                          AND IdUsuario = @IdUsuario
                          AND Estado = 'Activa';";

                    var matriculado = await cn.ExecuteScalarAsync<int>(
                        matriculaSql,
                        new { CursoId = cursoId, item.IdUsuario },
                        tx);

                    if (matriculado == 0)
                        return BadRequest($"El usuario {item.IdUsuario} no está matriculado en el curso.");

                    // Intentar actualizar
                    const string updateSql = @"
                        UPDATE dbo.Asistencia
                        SET EstadoAsistencia = @EstadoAsistencia
                        WHERE IdUsuario = @IdUsuario
                          AND IdCurso = @CursoId
                          AND Fecha = @Fecha;";

                    var filas = await cn.ExecuteAsync(
                        updateSql,
                        new
                        {
                            item.IdUsuario,
                            CursoId = cursoId,
                            request.Fecha,
                            item.EstadoAsistencia
                        },
                        tx);

                    if (filas == 0)
                    {
                        // Insertar si no existe
                        const string insertSql = @"
                            INSERT INTO dbo.Asistencia
                                (IdUsuario, IdCurso, Fecha, EstadoAsistencia)
                            VALUES (@IdUsuario, @CursoId, @Fecha, @EstadoAsistencia);";

                        await cn.ExecuteAsync(
                            insertSql,
                            new
                            {
                                item.IdUsuario,
                                CursoId = cursoId,
                                request.Fecha,
                                item.EstadoAsistencia
                            },
                            tx);
                    }
                }

                tx.Commit();
                return Ok(new { success = true, total = request.Items.Count });
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/instructores/{instructorId}/cursos/{cursoId}/calificaciones
        [HttpPost("{instructorId:int}/cursos/{cursoId:int}/calificaciones")]
        public async Task<IActionResult> RegistrarCalificaciones(
            int instructorId,
            int cursoId,
            [FromBody] CalificacionesRequestDto request)
        {
            if (request is null || request.Items.Count == 0)
                return BadRequest("No se enviaron calificaciones.");

            await using var cn = new SqlConnection(_connectionString);
            await cn.OpenAsync();

            var pertenece = await CursoPerteneceAInstructor(cn, cursoId, instructorId);
            if (!pertenece)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "El curso no pertenece al instructor.");
            }

            using var tx = cn.BeginTransaction();

            try
            {
                foreach (var item in request.Items)
                {
                    // Validar que el estudiante esté matriculado en el curso
                    const string matriculaSql = @"
                        SELECT COUNT(1)
                        FROM dbo.Matricula
                        WHERE IdCurso = @CursoId
                          AND IdUsuario = @IdUsuario
                          AND Estado = 'Activa';";

                    var matriculado = await cn.ExecuteScalarAsync<int>(
                        matriculaSql,
                        new { CursoId = cursoId, item.IdUsuario },
                        tx);

                    if (matriculado == 0)
                        return BadRequest($"El usuario {item.IdUsuario} no está matriculado en el curso.");

                    // Actualizar historial
                    const string updateSql = @"
                        UPDATE dbo.Historial
                        SET Calificacion = @Calificacion,
                            Estado_Curso = 'En Curso'
                        WHERE IdCurso = @CursoId
                          AND IdUsuario = @IdUsuario;";

                    var filas = await cn.ExecuteAsync(
                        updateSql,
                        new
                        {
                            item.Calificacion,
                            CursoId = cursoId,
                            item.IdUsuario
                        },
                        tx);

                    if (filas == 0)
                    {
                        const string insertSql = @"
                            INSERT INTO dbo.Historial
                                (IdUsuario, IdCurso, Calificacion, Estado_Curso)
                            VALUES (@IdUsuario, @CursoId, @Calificacion, 'En Curso');";

                        await cn.ExecuteAsync(
                            insertSql,
                            new
                            {
                                item.IdUsuario,
                                CursoId = cursoId,
                                item.Calificacion
                            },
                            tx);
                    }
                }

                tx.Commit();
                return Ok(new { success = true, total = request.Items.Count });
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private static async Task<bool> CursoPerteneceAInstructor(SqlConnection cn, int cursoId, int instructorId)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.Curso
                WHERE IdCurso = @CursoId
                  AND IdInstructor = @InstructorId;";

            var filas = await cn.ExecuteScalarAsync<int>(sql, new { CursoId = cursoId, InstructorId = instructorId });
            return filas > 0;
        }
    }
}

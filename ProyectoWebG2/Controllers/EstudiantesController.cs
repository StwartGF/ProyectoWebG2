using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoWebG2.Models;
using System.Net.Http.Json;

namespace ProyectoWebG2.Controllers
{
    public class EstudiantesController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _configuration;

        public EstudiantesController(
            IHttpClientFactory factory,
            IConfiguration configuration)
        {
            _factory = factory;
            _configuration = configuration;
        }


        [Seguridad]
        public IActionResult Index()
        {
            ViewBag.NombreUsuario =
                HttpContext.Session.GetString("NombreUsuario") ?? "Estudiante";

            return View();
        }


        [Seguridad]
        public async Task<IActionResult> CursosDisponibles()
        {
            try
            {
                using var http = _factory.CreateClient();

                var url = _configuration["Valores:UrlAPI"]
                          + "api/Estudiantes/CursosDisponibles";

                var response = await http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var cursos =
                        await response.Content.ReadFromJsonAsync<List<CursoDisponibleVM>>();

                    return View(cursos ?? new List<CursoDisponibleVM>());
                }

                TempData["Error"] = "No se pudieron cargar los cursos disponibles.";
                return View(new List<CursoDisponibleVM>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar cursos: {ex.Message}";
                return View(new List<CursoDisponibleVM>());
            }
        }


        [Seguridad]
        public async Task<IActionResult> ObtenerHorarios(int idCurso)
        {
            try
            {
                using var http = _factory.CreateClient();

                var url = _configuration["Valores:UrlAPI"]
                          + $"api/Estudiantes/HorariosCurso/{idCurso}";

                var response = await http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var horarios =
                        await response.Content.ReadFromJsonAsync<List<HorarioVM>>();

                    return Json(horarios ?? new List<HorarioVM>());
                }

                return Json(new List<HorarioVM>());
            }
            catch
            {
                return Json(new List<HorarioVM>());
            }
        }


        [HttpPost]
        [Seguridad]
        public async Task<IActionResult> Matricular([FromBody] MatricularRequestVM model)
        {
            int? idUsuario =
                HttpContext.Session.GetInt32("ConsecutivoUsuario");

            if (idUsuario == null || idUsuario == 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Sesión inválida. Inicie sesión nuevamente."
                });
            }

            try
            {
                using var http = _factory.CreateClient();

                var url = _configuration["Valores:UrlAPI"]
                          + "api/Estudiantes/Matricular";

                var payload = new
                {
                    IdUsuario = idUsuario.Value,
                    IdCurso = model.IdCurso,
                    IdHorario = model.IdHorario
                };

                var response = await http.PostAsJsonAsync(url, payload);

                var result = await response.Content.ReadFromJsonAsync<object>();
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al conectar con la API: " + ex.Message
                });
            }
        }

        [Seguridad]
        public async Task<IActionResult> CursosMatriculados()
        {
            int? idUsuario =
                HttpContext.Session.GetInt32("ConsecutivoUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                using var http = _factory.CreateClient();

                var url = _configuration["Valores:UrlAPI"]
                          + $"api/Estudiantes/CursosMatriculados/{idUsuario}";

                var response = await http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var cursos =
                        await response.Content.ReadFromJsonAsync<List<CursoMatriculadoVM>>();

                    return View(cursos ?? new List<CursoMatriculadoVM>());
                }

                TempData["Error"] = "No se pudieron cargar los cursos matriculados.";
                return View(new List<CursoMatriculadoVM>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(new List<CursoMatriculadoVM>());
            }
        }


        [HttpPost]
        [Seguridad]
        public async Task<IActionResult> Renunciar(int idCurso)
        {
            int? idUsuario =
                HttpContext.Session.GetInt32("ConsecutivoUsuario");

            if (idUsuario == null)
            {
                return Json(new { success = false, message = "Sesión inválida." });
            }

            try
            {
                using var http = _factory.CreateClient();

                var url = _configuration["Valores:UrlAPI"]
                          + $"api/Estudiantes/Desmatricular/{idUsuario}/{idCurso}";

                var response = await http.PostAsync(url, null);

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = "Error en el servidor." });
                }

                var result = await response.Content.ReadFromJsonAsync<int>();

                return result switch
                {
                    1 => Json(new { success = true, message = "Curso renunciado correctamente." }),
                    -1 => Json(new { success = false, message = "No estás matriculado en este curso." }),
                    _ => Json(new { success = false, message = "No se pudo renunciar al curso." })
                };
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        [Seguridad]
        public async Task<IActionResult> Calificaciones()
        {
            int? idUsuario =
                HttpContext.Session.GetInt32("ConsecutivoUsuario");

            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                using var http = _factory.CreateClient();

                var url = _configuration["Valores:UrlAPI"]
                          + $"api/Estudiantes/Calificaciones/{idUsuario}";

                var response = await http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var calificaciones =
                        await response.Content.ReadFromJsonAsync<List<CalificacionCursoVM>>();

                    return View(calificaciones ?? new List<CalificacionCursoVM>());
                }

                TempData["Error"] = "No se pudieron cargar las calificaciones.";
                return View(new List<CalificacionCursoVM>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(new List<CalificacionCursoVM>());
            }
        }
    }
}


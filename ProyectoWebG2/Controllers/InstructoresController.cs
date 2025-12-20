using Microsoft.AspNetCore.Mvc;
using ProyectoWebG2.Models;
using System.Net.Http.Json;

namespace ProyectoWebG2.Controllers
{
    public class InstructoresController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public InstructoresController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private int? InstructorId => HttpContext.Session.GetInt32("ConsecutivoUsuario");

        private IActionResult RedireccionLogin() => new RedirectToActionResult("Login", "Home", null);

        [Seguridad]
        [SoloInstructor]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var instructorId = InstructorId;
            if (instructorId is null)
            {
                return RedireccionLogin();
            }

            var client = _factory.CreateClient("api");
            try
            {
                var response = await client.GetAsync($"api/Instructores/{instructorId}/cursos");
                if (response.IsSuccessStatusCode)
                {
                    var cursos = await response.Content.ReadFromJsonAsync<List<CursoInstructorVM>>() ?? new List<CursoInstructorVM>();
                    return View(cursos);
                }

                TempData["Error"] = $"No se pudieron cargar los cursos. Código {(int)response.StatusCode}.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudieron cargar los cursos. {ex.Message}";
            }

            return View(new List<CursoInstructorVM>());
        }

        [Seguridad]
        [SoloInstructor]
        [HttpGet]
        public async Task<IActionResult> Estudiantes(int cursoId)
        {
            var instructorId = InstructorId;
            if (instructorId is null)
            {
                return RedireccionLogin();
            }

            var client = _factory.CreateClient("api");
            try
            {
                var response = await client.GetAsync($"api/Instructores/{instructorId}/cursos/{cursoId}/estudiantes");
                if (response.IsSuccessStatusCode)
                {
                    var estudiantes = await response.Content.ReadFromJsonAsync<List<EstudianteCursoVM>>() ?? new List<EstudianteCursoVM>();
                    ViewBag.CursoId = cursoId;
                    return View(estudiantes);
                }

                TempData["Error"] = $"No se pudieron cargar los estudiantes. Código {(int)response.StatusCode}.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudieron cargar los estudiantes. {ex.Message}";
            }

            ViewBag.CursoId = cursoId;
            return View(new List<EstudianteCursoVM>());
        }

        [Seguridad]
        [SoloInstructor]
        [HttpGet]
        public async Task<IActionResult> Asistencia(int cursoId)
        {
            var instructorId = InstructorId;
            if (instructorId is null)
            {
                return RedireccionLogin();
            }

            var client = _factory.CreateClient("api");
            try
            {
                var response = await client.GetAsync($"api/Instructores/{instructorId}/cursos/{cursoId}/estudiantes");
                if (response.IsSuccessStatusCode)
                {
                    var estudiantes = await response.Content.ReadFromJsonAsync<List<EstudianteCursoVM>>() ?? new List<EstudianteCursoVM>();

                    var form = new AsistenciaFormVM
                    {
                        CursoId = cursoId,
                        Fecha = DateTime.Today,
                        Items = estudiantes.Select(e => new AsistenciaItemVM
                        {
                            IdUsuario = e.IdUsuario,
                            NombreCompleto = $"{e.Nombre} {e.Apellidos}".Trim(),
                            EstadoAsistencia = "Presente"
                        }).ToList()
                    };

                    return View(form);
                }

                TempData["Error"] = $"No se pudieron cargar los estudiantes del curso. Código {(int)response.StatusCode}.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudieron cargar los estudiantes del curso. {ex.Message}";
            }

            return View(new AsistenciaFormVM
            {
                CursoId = cursoId,
                Fecha = DateTime.Today,
                Items = new List<AsistenciaItemVM>()
            });
        }

        [Seguridad]
        [SoloInstructor]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asistencia(AsistenciaFormVM form)
        {
            var instructorId = InstructorId;
            if (instructorId is null)
            {
                return RedireccionLogin();
            }

            if (form.Items == null || !form.Items.Any())
            {
                TempData["Error"] = "Debe seleccionar estados de asistencia.";
                return RedirectToAction(nameof(Asistencia), new { cursoId = form.CursoId });
            }

            var client = _factory.CreateClient("api");
            var payload = new
            {
                fecha = form.Fecha,
                items = form.Items.Select(i => new
                {
                    idUsuario = i.IdUsuario,
                    estadoAsistencia = i.EstadoAsistencia
                })
            };

            try
            {
                var response = await client.PostAsJsonAsync(
                    $"api/Instructores/{instructorId}/cursos/{form.CursoId}/asistencia",
                    payload);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Msg"] = "Asistencia guardada";
                }
                else
                {
                    TempData["Error"] = $"No se pudo guardar la asistencia. Código {(int)response.StatusCode}.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudo guardar la asistencia. {ex.Message}";
            }

            return RedirectToAction(nameof(Estudiantes), new { cursoId = form.CursoId });
        }

        [Seguridad]
        [SoloInstructor]
        [HttpGet]
        public async Task<IActionResult> Calificaciones(int cursoId)
        {
            var instructorId = InstructorId;
            if (instructorId is null)
            {
                return RedireccionLogin();
            }

            var client = _factory.CreateClient("api");
            try
            {
                var response = await client.GetAsync($"api/Instructores/{instructorId}/cursos/{cursoId}/estudiantes");
                if (response.IsSuccessStatusCode)
                {
                    var estudiantes = await response.Content.ReadFromJsonAsync<List<EstudianteCursoVM>>() ?? new List<EstudianteCursoVM>();

                    var form = new CalificacionesFormVM
                    {
                        CursoId = cursoId,
                        Items = estudiantes.Select(e => new CalificacionItemVM
                        {
                            IdUsuario = e.IdUsuario,
                            NombreCompleto = $"{e.Nombre} {e.Apellidos}".Trim(),
                            Calificacion = null
                        }).ToList()
                    };

                    return View(form);
                }

                TempData["Error"] = $"No se pudieron cargar los estudiantes del curso. Código {(int)response.StatusCode}.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudieron cargar los estudiantes del curso. {ex.Message}";
            }

            return View(new CalificacionesFormVM
            {
                CursoId = cursoId,
                Items = new List<CalificacionItemVM>()
            });
        }

        [Seguridad]
        [SoloInstructor]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calificaciones(CalificacionesFormVM form)
        {
            var instructorId = InstructorId;
            if (instructorId is null)
            {
                return RedireccionLogin();
            }

            if (form.Items == null || !form.Items.Any())
            {
                TempData["Error"] = "No se enviaron calificaciones para procesar.";
                return RedirectToAction(nameof(Calificaciones), new { cursoId = form.CursoId });
            }

            var client = _factory.CreateClient("api");
            var payload = new
            {
                items = form.Items.Select(i => new
                {
                    idUsuario = i.IdUsuario,
                    calificacion = i.Calificacion
                })
            };

            try
            {
                var response = await client.PostAsJsonAsync(
                    $"api/Instructores/{instructorId}/cursos/{form.CursoId}/calificaciones",
                    payload);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Msg"] = "Calificaciones guardadas";
                }
                else
                {
                    TempData["Error"] = $"No se pudieron guardar las calificaciones. Código {(int)response.StatusCode}.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"No se pudieron guardar las calificaciones. {ex.Message}";
            }

            return RedirectToAction(nameof(Estudiantes), new { cursoId = form.CursoId });
        }
    }
}

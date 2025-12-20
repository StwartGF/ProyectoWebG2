using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoWebG2.Models;
using System.Net.Http.Json;
using System.Linq;

namespace ProyectoWebG2.Controllers
{
    public class CursosController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public CursosController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private HttpClient GetApiClient() => _factory.CreateClient("api");

        // DTO interno para leer instructores desde la API
        private sealed class InstructorComboDto
        {
            public int Id { get; set; }
            public string Cedula { get; set; } = string.Empty;
            public string Nombre { get; set; } = string.Empty;
            public string Apellidos { get; set; } = string.Empty;

            public string NombreCompleto => $"{Nombre} {Apellidos}";
        }

        // ============================
        // Cargar instructores en combo
        // ============================
        private async Task CargarInstructoresAsync(int? idSeleccionado = null)
        {
            var client = GetApiClient();

            var lista = await client.GetFromJsonAsync<List<InstructorComboDto>>(
                "api/admin/instructores");

            var items = (lista ?? new List<InstructorComboDto>())
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.NombreCompleto} ({x.Cedula})",
                    Selected = idSeleccionado.HasValue && x.Id == idSeleccionado.Value
                })
                .ToList();

            // opción “Por asignar”
            items.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Por asignar",
                Selected = !idSeleccionado.HasValue
            });

            ViewBag.Instructores = items;
        }

        // ============================
        // LISTAR CURSOS
        // ============================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = GetApiClient();

            // 1) Traemos cursos
            var cursos = await client.GetFromJsonAsync<List<CursoVM>>("api/cursos")
                         ?? new List<CursoVM>();

            // 2) Traemos instructores para mostrar el nombre y para el combo del modal
            var instructores = await client.GetFromJsonAsync<List<InstructorComboDto>>(
                                   "api/admin/instructores")
                               ?? new List<InstructorComboDto>();

            // Mapear nombre del instructor para mostrar en la tabla
            foreach (var c in cursos)
            {
                if (c.IdInstructor.HasValue)
                {
                    var inst = instructores.FirstOrDefault(i => i.Id == c.IdInstructor.Value);
                    c.NombreInstructor = inst?.NombreCompleto ?? "Por asignar";
                }
                else
                {
                    c.NombreInstructor = "Por asignar";
                }
            }

            // Combo para el modal (sin selección inicial)
            var items = instructores
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.NombreCompleto} ({x.Cedula})"
                })
                .ToList();

            items.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Por asignar"
            });

            ViewBag.Instructores = items;

            return View(cursos);
        }

        // ============================
        // CREAR (GET) – página normal
        // ============================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarInstructoresAsync();

            var vm = new CursoVM
            {
                FechaDeInicio = DateTime.Today,
                FechaDeFinalizacion = DateTime.Today.AddMonths(1)
            };

            return View(vm);
        }

        // ============================
        // CREAR (POST)
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CursoVM vm)
        {
            if (!ModelState.IsValid)
            {
                await CargarInstructoresAsync(vm.IdInstructor);
                return View(vm);
            }

            var client = GetApiClient();
            var res = await client.PostAsJsonAsync("api/cursos", vm);

            if (res.IsSuccessStatusCode)
            {
                TempData["Msg"] = "Curso creado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            var contenido = await res.Content.ReadAsStringAsync();
            TempData["Error"] =
                $"No se pudo crear el curso. Código: {(int)res.StatusCode} - {res.StatusCode}. " +
                $"Detalle API: {contenido}";

            await CargarInstructoresAsync(vm.IdInstructor);
            return View(vm);
        }

        // ============================
        // EDITAR (POST) – SOLO MODAL
        // ============================
        // El formulario del modal manda TODOS los campos de CursoVM,
        // incluyendo IdCurso, y aquí llamamos al API con PUT.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CursoVM vm)
        {
            if (!ModelState.IsValid)
            {
                // Si algo falla, volvemos al Index con un mensaje
                TempData["Error"] = "Revise los datos del curso.";
                return RedirectToAction(nameof(Index));
            }

            var client = GetApiClient();

            var res = await client.PutAsJsonAsync($"api/cursos/{vm.IdCurso}", vm);

            if (res.IsSuccessStatusCode ||
                res.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                TempData["Msg"] = "Curso actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            var contenido = await res.Content.ReadAsStringAsync();
            TempData["Error"] =
                $"No se pudo actualizar el curso. Código: {(int)res.StatusCode} - {res.StatusCode}. " +
                $"Detalle API: {contenido}";

            return RedirectToAction(nameof(Index));
        }
    }
}


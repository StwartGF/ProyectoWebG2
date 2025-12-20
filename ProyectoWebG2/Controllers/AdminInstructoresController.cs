using Microsoft.AspNetCore.Mvc;
using ProyectoWebG2.Models;
using System.Net.Http.Json;
using System.Security.Cryptography;

namespace ProyectoWebG2.Controllers
{
    public class AdminInstructoresController : Controller
    {
        private readonly IHttpClientFactory _factory;

        public AdminInstructoresController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        // =========================
        // LISTAR INSTRUCTORES
        // =========================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = _factory.CreateClient("api");
            var apiList = await client.GetFromJsonAsync<List<InstructorVM>>(
                "api/admin/instructores");

            var model = apiList ?? new List<InstructorVM>();

            return View(model);
        }

        // =========================
        // CREAR (GET)
        // =========================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // =========================
        // CREAR (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InstructorVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var client = _factory.CreateClient("api");

            // Hasheamos la contraseña
            string contrasenaHash = HashPassword(vm.ContrasenaHash);

            var payload = new
            {
                Cedula = vm.Cedula,
                Nombre = vm.Nombre,
                Apellidos = vm.Apellidos,
                Telefono = vm.Telefono,
                Correo = vm.Correo,
                ContrasenaHash = contrasenaHash
            };

            // POST: https://localhost:7238/api/admin/instructores
            var res = await client.PostAsJsonAsync("api/admin/instructores", payload);

            if (res.IsSuccessStatusCode)
            {
                TempData["Msg"] = "Instructor creado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            var contenido = await res.Content.ReadAsStringAsync();
            TempData["Error"] =
                $"No se pudo crear el instructor. Código: {(int)res.StatusCode} - {res.StatusCode}. " +
                $"Detalle API: {contenido}";

            return View(vm);
        }

        // =========================
        // EDITAR (GET)
        // /Instructores/Edit/5
        // =========================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = _factory.CreateClient("api");

            // GET: https://localhost:7238/api/admin/instructores/{id}
            var dto = await client.GetFromJsonAsync<InstructorVM>(
                $"api/admin/instructores/{id}");

            if (dto == null)
                return NotFound();

            // Para no mostrar ni tocar la contraseña desde aquí
            dto.ContrasenaHash = string.Empty;

            return View(dto);
        }

        // =========================
        // EDITAR (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InstructorVM vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos inválidos. Verifique la información del instructor.";
                return RedirectToAction(nameof(Index));
            }

            var client = _factory.CreateClient("api");

            var payload = new
            {
                Cedula = vm.Cedula,
                Nombre = vm.Nombre,
                Apellidos = vm.Apellidos,
                Telefono = vm.Telefono,
                Correo = vm.Correo,
                ContrasenaHash = vm.ContrasenaHash // la API PUT realmente no la usa
            };

            var res = await client.PutAsJsonAsync($"api/admin/instructores/{id}", payload);

            if (res.IsSuccessStatusCode)
            {
                TempData["Msg"] = "Instructor actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            var contenido = await res.Content.ReadAsStringAsync();
            TempData["Error"] =
                $"No se pudo actualizar el instructor. Código: {(int)res.StatusCode} - {res.StatusCode}. " +
                $"Detalle API: {contenido}";

            return RedirectToAction(nameof(Index));
        }


        // =========================
        // HASH DE CONTRASEÑA
        // =========================
        private static string HashPassword(string password)
        {
            const int iterations = 100_000;
            const int saltSize = 16;
            const int keySize = 32;

            var salt = RandomNumberGenerator.GetBytes(saltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                keySize
            );

            return $"v1${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }
    }
}





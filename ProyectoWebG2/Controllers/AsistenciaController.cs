using Microsoft.AspNetCore.Mvc;
using ProyectoWebG2.Models;
using System.Net.Http.Json;

namespace ProyectoWebG2.Controllers
{
    public class AsistenciaController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private const string ApiBaseUrl = "https://localhost:7238/api/Asistencias"; // puerto de tu API

        public AsistenciaController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        private HttpClient GetClient() => _factory.CreateClient("api");

        // GET: /Asistencia
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var data = await client.GetFromJsonAsync<List<AsistenciaModel>>(ApiBaseUrl);
            return View(data ?? new List<AsistenciaModel>());
        }

        // GET: /Asistencia/Create
        public IActionResult Create()
        {
            return View(new AsistenciaModel
            {
                Fecha = DateTime.Today
            });
        }

        // POST: /Asistencia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AsistenciaModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var client = GetClient();
            var response = await client.PostAsJsonAsync(ApiBaseUrl, modelo);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error al guardar la asistencia.");
                return View(modelo);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Asistencia/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = GetClient();
            var item = await client.GetFromJsonAsync<AsistenciaModel>($"{ApiBaseUrl}/{id}");
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Asistencia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AsistenciaModel modelo)
        {
            if (id != modelo.IdAsistencia) return BadRequest();

            if (!ModelState.IsValid)
                return View(modelo);

            var client = GetClient();
            var response = await client.PutAsJsonAsync($"{ApiBaseUrl}/{id}", modelo);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar la asistencia.");
                return View(modelo);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Asistencia/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            var item = await client.GetFromJsonAsync<AsistenciaModel>($"{ApiBaseUrl}/{id}");
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Asistencia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = GetClient();
            var response = await client.DeleteAsync($"{ApiBaseUrl}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Error al eliminar la asistencia.");
                // Volvemos a cargar el registro para mostrar algo
                var item = await client.GetFromJsonAsync<AsistenciaModel>($"{ApiBaseUrl}/{id}");
                return View("Delete", item);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

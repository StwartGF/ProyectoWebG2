using Microsoft.AspNetCore.Mvc;
using ProyectoWebG2.Models;
using System.Text.Json;

namespace ProyectoWebG2.Controllers
{
    public class CalificacionesController : Controller
    {
        private readonly IHttpClientFactory _factory;
        public CalificacionesController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _factory.CreateClient("api");
            const string endpoint = "api/admin/calificaciones";

            var response = await client.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = $"No se pudieron obtener las calificaciones. " +
                                  $"Código {(int)response.StatusCode} ({response.StatusCode}) al llamar '{endpoint}'.";
                return View(new List<CalificacionVM>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<CalificacionVM>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(data ?? new List<CalificacionVM>());
        }
    }
}

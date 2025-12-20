using Microsoft.AspNetCore.Mvc;
using ProyectoWebG2.Models;
using System.Text.Json;

namespace ProyectoWebG2.Controllers
{
    public class MatriculaController : Controller
    {
        private readonly IHttpClientFactory _factory;
        public MatriculaController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _factory.CreateClient("api");
            const string endpoint = "api/admin/matriculas";

            var response = await client.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = $"No se pudieron obtener las matrículas. " +
                                  $"Código {(int)response.StatusCode} ({response.StatusCode}) al llamar '{endpoint}'.";
                return View(new List<MatriculaVM>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<MatriculaVM>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(data ?? new List<MatriculaVM>());
        }
    }
}

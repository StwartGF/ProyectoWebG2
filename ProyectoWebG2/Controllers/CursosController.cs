using Microsoft.AspNetCore.Mvc;
using ProyectoWebG2.Models;

namespace ProyectoWebG2.Controllers
{
    public class CursosController : Controller
    {
        private readonly HttpClient _httpClient;

        public CursosController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("api");
        }

        public async Task<IActionResult> Index()
        {
            var cursos = await _httpClient.GetFromJsonAsync<List<Curso>>("api/cursos");
            return View(cursos ?? new List<Curso>());
        }
    }
}

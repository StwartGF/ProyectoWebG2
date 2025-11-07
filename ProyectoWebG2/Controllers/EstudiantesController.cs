using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoWebG2.Models;
using System.Net.Http.Json;

namespace ProyectoWebG2.Controllers
{
    public class EstudiantesController : Controller
    {
        private readonly IHttpClientFactory _factory;
        public EstudiantesController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        // GET: EstudiantesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: EstudiantesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EstudiantesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EstudiantesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EstudiantesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EstudiantesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EstudiantesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EstudiantesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> Calificaciones()
        {
            int? idUsuario = HttpContext.Session.GetInt32("ConsecutivoUsuario");
            if (idUsuario == null)
                return RedirectToAction("Login", "Home");

            var client = _factory.CreateClient("api");
            var response = await client.GetAsync($"api/Estudiantes/Calificaciones/{idUsuario}");
            var data = await response.Content.ReadFromJsonAsync<List<CalificacionCursoVM>>();
            return View(data ?? new List<CalificacionCursoVM>());
        }
    }
}

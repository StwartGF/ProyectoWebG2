using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProyectoWebG2Api.Controllers
{
    public class EstudiantesController : Controller
    {
        private readonly IConfiguration _cfg;

        public EstudiantesController(IConfiguration cfg)
        {
            _cfg = cfg;
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

        [HttpGet("Calificaciones/{idUsuario}")]
        public IActionResult GetCalificaciones(int idUsuario)
        {
            using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);
            var p = new DynamicParameters();
            p.Add("@IdUsuario", idUsuario);
            var result = cn.Query("ObtenerCalificacionesPorUsuario", p, commandType: CommandType.StoredProcedure);
            return Ok(result);
        }
    }
}

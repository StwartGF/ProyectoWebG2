using Microsoft.AspNetCore.Mvc;

namespace ProyectoWebG2Api.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

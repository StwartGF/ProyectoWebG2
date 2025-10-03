using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoWebG2.Models;

namespace ProyectoWebG2.Controllers
{
    public class HomeController : Controller
    {
        // /  y  /inicio  -> Index
        [HttpGet("")]
        [HttpGet("inicio")]
        public IActionResult Index()
        {
            return View();
        }

        // /login  -> Login (tu pantalla externa)
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Registro";
            return View(); // Views/Home/Register.cshtml
        }

        // /recuperar
        [HttpGet]
        [Route("recuperar")]
        public IActionResult Recuperar()
        {
            return View();
        }

    }
}

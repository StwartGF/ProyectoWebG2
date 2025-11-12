using Microsoft.AspNetCore.Mvc;
using ProyectoWebG2.Models;
using System.Net.Http.Json;

namespace ProyectoWebG2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _factory;
        public HomeController(IConfiguration configuration, IHttpClientFactory factory)
        {
            _configuration = configuration;
            _factory = factory;
        }

        #region Iniciar Sesión

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginPost(LoginVM usuario)
        {
            using var http = _factory.CreateClient();
            var url = _configuration["Valores:UrlAPI"] + "Home/IniciarSesion";
            var res = await http.PostAsJsonAsync(url, usuario);

            if (!res.IsSuccessStatusCode)
            {
                TempData["Error"] = "Credenciales inválidas.";
                return View("Login", usuario);
            }

            // Marca sesión mínima
            HttpContext.Session.SetString("IsAuth", "1");
            HttpContext.Session.SetString("Email", usuario.CorreoElectronico ?? string.Empty);

            return RedirectToAction("Index", "Home");
        }


        #endregion


        #region Crear Usuarios

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(UsuarioModel usuario)
        {
            using (var context = _factory.CreateClient())
            {
                var urlApi = _configuration["Valores:UrlAPI"] + "Home/Registro";
                var resultado = context.PostAsJsonAsync(urlApi, usuario).Result;

                if (!string.Equals(usuario.Contrasena, usuario.ConfirmarContrasena))
                {
                    TempData["Error"] = "Las contraseñas no coinciden.";
                    ModelState.AddModelError(nameof(usuario.ConfirmarContrasena), "Debe coincidir con la contraseña.");
                    return View("Registro", usuario);
                }


                if (resultado.IsSuccessStatusCode)
                {
                    var datosApi = resultado.Content.ReadFromJsonAsync<int>().Result;

                    if (datosApi > 0)
                        return RedirectToAction("Login", "Home");
                }

                ViewBag.Mensaje = "No se ha registrado la información";
                return View();
            }
        }

        #endregion

        #region Recuperar Acceso

        [HttpGet]
        public IActionResult RecuperarAcceso()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RecuperarAcceso(UsuarioModel usuario)
        {
            using (var context = _factory.CreateClient())
            {
                var urlApi = _configuration["Valores:UrlAPI"] + "Home/RecuperarAcceso?CorreoElectronico=" + usuario.CorreoElectronico;
                var resultado = context.GetAsync(urlApi).Result;

                if (resultado.IsSuccessStatusCode)
                {
                    var datosApi = resultado.Content.ReadFromJsonAsync<UsuarioModel>().Result;

                    if (datosApi != null)
                        return RedirectToAction("Login", "Home");
                }

                ViewBag.Mensaje = "No se ha recuperado el acceso";
                return View();
            }
        }

        #endregion

        [Seguridad]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Seguridad]
        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }

    }
}


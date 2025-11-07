using Microsoft.AspNetCore.Mvc;
using ProyectoWebG2.Models;
using System.Net.Http.Json;

namespace ProyectoWebG2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _cfg;
        private readonly IHttpClientFactory _factory;

        public HomeController(IConfiguration cfg, IHttpClientFactory factory)
        {
            _cfg = cfg;
            _factory = factory;
        }

        // DTO interno para deserializar lo que devuelve el API al iniciar sesión
        private sealed class ApiSesionResponse
        {
            public int ConsecutivoUsuario { get; set; }
            public string Nombre { get; set; } = "";
            public string NombrePerfil { get; set; } = "";
        }

        private string ApiBase => _cfg["Valores:UrlAPI"]!.TrimEnd('/') + "/";

        // ============================================
        // INDEX (GET)
        // - Si NO hay sesión -> muestra Login
        // - Si hay sesión     -> muestra tu dashboard (Views/Home/Index.cshtml)
        // ============================================


        //[HttpGet("")]
        //public IActionResult Index()
        //{
        //  var logged = HttpContext.Session.GetInt32("ConsecutivoUsuario") != null;
        //if (!logged)
        //  return View("Login", new LoginVM()); // formulario

        //            return View("Index"); // tu dashboard interno
        //      }
        //-
        [HttpGet("")]
        public IActionResult Index()
        {
            // ⚠️ Solo para pruebas
            return View("Index");
        }
//-

        // ============================================
        // LOGIN (GET) - ruta dedicada al formulario
        // Siempre muestra el login, útil para enlaces directos /login
        // ============================================
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("Login", new LoginVM());
        }

        // ============================================
        // LOGIN (POST)
        // Llama a: POST {UrlAPI}/Home/IniciarSesion
        // ============================================
        [HttpPost("login")]
        public async Task<IActionResult> LoginPost(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Complete los campos requeridos.";
                return View("Login", vm);
            }

            try
            {
                using var http = _factory.CreateClient();
                var url = $"{ApiBase}Home/IniciarSesion";

                var payload = new { CorreoElectronico = vm.CorreoElectronico, Contrasenna = vm.Contrasena };
                var res = await http.PostAsJsonAsync(url, payload);

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Credenciales inválidas.";
                    return View("Login", vm);
                }

                var data = await res.Content.ReadFromJsonAsync<ApiSesionResponse>();
                if (data is null)
                {
                    TempData["Error"] = "Respuesta inválida del servidor.";
                    return View("Login", vm);
                }

                // Guardar sesión para layouts internos
                HttpContext.Session.SetInt32("ConsecutivoUsuario", data.ConsecutivoUsuario);
                HttpContext.Session.SetString("NombreUsuario", data.Nombre);
                HttpContext.Session.SetString("NombrePerfil", string.IsNullOrWhiteSpace(data.NombrePerfil) ? "Estudiante" : data.NombrePerfil);

                // Ahora redirige a Index (dashboard interno)
                return RedirectToAction("Index");
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "No se pudo contactar el servidor. Verifique que la API esté en ejecución.";
                return View("Login", vm);
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error inesperado.";
                return View("Login", vm);
            }
        }

        // ============================================
        // RECUPERAR (GET/POST) — sin cambios
        // ============================================
        [HttpGet("recuperar")]
        public IActionResult Recuperar() => View();

        [HttpPost("recuperar")]
        public async Task<IActionResult> RecuperarPost(UsuarioModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.CorreoElectronico))
            {
                TempData["Error"] = "Ingrese su correo.";
                return View("Recuperar", vm);
            }

            try
            {
                using var http = _factory.CreateClient();
                var url = $"{ApiBase}Home/RecuperarAcceso?CorreoElectronico={Uri.EscapeDataString(vm.CorreoElectronico)}";

                var res = await http.GetAsync(url);
                if (res.IsSuccessStatusCode)
                {
                    TempData["Msg"] = "Te enviamos un correo con la nueva contraseña.";
                    return RedirectToAction("Login"); // vuelve al login
                }

                TempData["Error"] = "Correo no encontrado.";
                return View("Recuperar", vm);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "No se pudo contactar el servidor. Verifique que la API esté en ejecución.";
                return View("Recuperar", vm);
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error inesperado.";
                return View("Recuperar", vm);
            }
        }

        // ============================================
        // CERRAR SESIÓN
        // ============================================
        [HttpGet("logout")]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            // Al volver a Index, como no hay sesión, mostrará el Login
            return RedirectToAction("Index");
        }
    }
}





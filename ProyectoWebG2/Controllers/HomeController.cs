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

        #region Iniciar Sesi�n

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
                TempData["Error"] = "Credenciales inv�lidas.";
                return View("Login", usuario);
            }

            // Marca sesi�n m�nima
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
                    TempData["Error"] = "Las contrase�as no coinciden.";
                    ModelState.AddModelError(nameof(usuario.ConfirmarContrasena), "Debe coincidir con la contrase�a.");
                    return View("Registro", usuario);
                }


                if (resultado.IsSuccessStatusCode)
                {
                    var datosApi = resultado.Content.ReadFromJsonAsync<int>().Result;

                    if (datosApi > 0)
                        return RedirectToAction("Login", "Home");
                }

                ViewBag.Mensaje = "No se ha registrado la informaci�n";
                return View();
            }
        }


        // ============================================
        // REGISTRO (GET)
        // Muestra el formulario de registro
        // ============================================
        [HttpGet("registro")]
        public IActionResult Registro()
        {
            return View("Register", new UsuarioModel());
        }

        // ============================================
        // REGISTRO (POST)
        // Llama a: POST {UrlAPI}/Home/Registro
        // ============================================
        [HttpPost("registro")]
        public async Task<IActionResult> RegistroPost(UsuarioModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Complete los campos requeridos.";
                return View("Register", vm);
            }

            try
            {
                using var http = _factory.CreateClient();
                var url = $"{ApiBase}Home/Registro";

                var payload = new {
                    Identificacion = vm.Identificacion,
                    Nombre = vm.Nombre,
                    Apellidos = vm.Apellidos,
                    CorreoElectronico = vm.CorreoElectronico,
                    Telefono = vm.Telefono,
                    Fecha_Nacimiento = vm.Fecha_Nacimiento,
                    NombreUsuario = vm.NombreUsuario,
                    Contrasenna = vm.Contrasena,
                    ContrasennaConfirmar = vm.ContrasenaConfirmar,
                    IdRol = 2
                };

                var res = await http.PostAsJsonAsync(url, payload);

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "No se pudo registrar. Verifique los datos o si ya existe el usuario/correo.";
                    return View("Register", vm);
                }

                TempData["Msg"] = "Registro exitoso. Ahora puedes iniciar sesión.";
                return RedirectToAction("Login");
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error inesperado.";
                return View("Register", vm);
            }
        }

        // ============================================
        // RECUPERAR (GET/POST) — sin cambios
        // ============================================
        [HttpGet("recuperar")]
        public IActionResult Recuperar() => View();


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


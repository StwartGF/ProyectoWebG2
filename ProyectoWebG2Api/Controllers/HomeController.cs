using Dapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MimeKit;
using SM_ProyectoAPI.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace SM_ProyectoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        private readonly IHostEnvironment _env;
        public HomeController(IConfiguration cfg, IHostEnvironment env)
        {
            _cfg = cfg;
            _env = env;
        }

        // POST api/Home/Registro
        [HttpPost("Registro")]
        public IActionResult Registro([FromBody] RegistroUsuarioRequestModel m)
        {
            if (!ModelState.IsValid) return BadRequest(-98);
            if (m.Contrasenna != m.ContrasennaConfirmar) return BadRequest(-99);

            using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);
            var p = new DynamicParameters();
            p.Add("@Identificacion", m.Identificacion);
            p.Add("@Nombre", m.Nombre);
            p.Add("@Apellidos", m.Apellidos);
            p.Add("@CorreoElectronico", m.CorreoElectronico.Trim());
            p.Add("@Telefono", string.IsNullOrWhiteSpace(m.Telefono) ? null : m.Telefono);
            p.Add("@Fecha_Nacimiento", m.Fecha_Nacimiento);
            p.Add("@NombreUsuario", m.NombreUsuario);

            // Hash en API (BCrypt)
            var hash = BCrypt.Net.BCrypt.HashPassword(m.Contrasenna);
            p.Add("@ContrasenaHash", hash);
            p.Add("@IdRol", m.IdRol ?? 2);

            var result = cn.Execute("Registro", p, commandType: CommandType.StoredProcedure);
            return Ok(result); // 1 OK, <0 duplicados
        }

        // POST api/Home/IniciarSesion
        [HttpPost("IniciarSesion")]
        public IActionResult IniciarSesion([FromBody] ValidarSesionRequestModel login)
        {
            if (!ModelState.IsValid) return BadRequest();

            using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);
            var p = new DynamicParameters();
            p.Add("@CorreoElectronico", login.CorreoElectronico);
            p.Add("@Contrasenna", login.Contrasenna); // ignorado por el SP; validamos en API

            // 1) Traemos al usuario + hash
            var row = cn.QueryFirstOrDefault<ValidarSesionResponse>(
                "ValidarInicioSesion", p, commandType: CommandType.StoredProcedure);

            if (row == null) return NotFound();

            // 2) Validamos el password comparando con hash
            if (!BCrypt.Net.BCrypt.Verify(login.Contrasenna, row.ContrasenaHash))
                return NotFound(); // credenciales inválidas

            // 3) No devolvemos el hash al MVC
            row.ContrasenaHash = "";

            return Ok(row);
        }

        // GET api/Home/RecuperarAcceso?CorreoElectronico=...
        [HttpGet("RecuperarAcceso")]
        public async Task<IActionResult> RecuperarAcceso([FromQuery] string CorreoElectronico)
        {
            if (string.IsNullOrWhiteSpace(CorreoElectronico)) return BadRequest("Correo requerido.");

            using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);

            // 1) Buscar usuario
            var pVal = new DynamicParameters();
            pVal.Add("@CorreoElectronico", CorreoElectronico.Trim());
            var usuario = cn.QueryFirstOrDefault<ValidarSesionResponse>(
                "ValidarUsuario", pVal, commandType: CommandType.StoredProcedure);

            if (usuario == null) return NotFound();

            // 2) Generar nueva contraseña y su hash
            var nueva = GenerarContrasena();
            var hash = BCrypt.Net.BCrypt.HashPassword(nueva);

            // 3) Actualizar en BD
            var pUpd = new DynamicParameters();
            pUpd.Add("@ConsecutivoUsuario", usuario.ConsecutivoUsuario);
            pUpd.Add("@ContrasenaHash", hash);
            var actualizado = cn.Execute("ActualizarContrasenna", pUpd, commandType: CommandType.StoredProcedure);
            if (actualizado <= 0) return StatusCode(500, "No se pudo actualizar la contraseña.");

            // 4) Cargar plantilla y enviar correo
            var ruta = Path.Combine(_env.ContentRootPath, "PlantillaCorreo.html");
            var html = System.IO.File.ReadAllText(ruta, Encoding.UTF8)
                        .Replace("{{Nombre}}", usuario.Nombre)
                        .Replace("{{Contrasenna}}", nueva);

            await EnviarCorreo(usuario.CorreoElectronico, "Recuperación de acceso", html);
            return Ok(usuario);
        }

        // === Helpers ===
        private string GenerarContrasena()
        {
            const int N = 8;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var data = new byte[N];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(data);
            var sb = new StringBuilder(N);
            for (int i = 0; i < N; i++) sb.Append(chars[data[i] % chars.Length]);
            return sb.ToString();
        }

        private async Task EnviarCorreo(string destinatario, string asunto, string html)
        {
            var from = _cfg["Valores:CorreoSMTP"];
            var pass = _cfg["Valores:ContrasennaSMTP"];
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("SM Web", from));
            msg.To.Add(MailboxAddress.Parse(destinatario));
            msg.Subject = asunto;
            msg.Body = new TextPart("html") { Text = html };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(from, pass);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
        }
    }
}


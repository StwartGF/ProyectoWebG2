using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SM_ProyectoAPI.Models;
using System.Data;

namespace SM_ProyectoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        public UsuarioController(IConfiguration cfg) { _cfg = cfg; }

        [HttpGet("ConsultarUsuario")]
        public IActionResult ConsultarUsuario(int ConsecutivoUsuario)
        {
            using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);
            var p = new DynamicParameters();
            p.Add("@ConsecutivoUsuario", ConsecutivoUsuario);
            var row = cn.QueryFirstOrDefault<ValidarSesionResponse>(
                "ConsultarUsuario", p, commandType: CommandType.StoredProcedure);
            return row is null ? NotFound() : Ok(row);
        }

        [HttpPut("ActualizarPerfil")]
        public IActionResult ActualizarPerfil(PerfilRequestModel m)
        {
            using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);
            var p = new DynamicParameters();
            p.Add("@ConsecutivoUsuario", m.ConsecutivoUsuario);
            p.Add("@Identificacion", m.Identificacion);
            p.Add("@Nombre", m.Nombre);
            p.Add("@Apellidos", m.Apellidos);
            p.Add("@CorreoElectronico", m.CorreoElectronico);
            p.Add("@Telefono", m.Telefono);
            p.Add("@Fecha_Nacimiento", m.Fecha_Nacimiento);
            p.Add("@NombreUsuario", m.NombreUsuario);

            var res = cn.Execute("ActualizarPerfil", p, commandType: CommandType.StoredProcedure);
            return Ok(res);
        }

        [HttpPut("ActualizarSeguridad")]
        public IActionResult ActualizarSeguridad(SeguridadRequestModel m)
        {
            using var cn = new SqlConnection(_cfg["ConnectionStrings:BDConnection"]);
            var hash = BCrypt.Net.BCrypt.HashPassword(m.Contrasenna);

            var p = new DynamicParameters();
            p.Add("@ConsecutivoUsuario", m.ConsecutivoUsuario);
            p.Add("@ContrasenaHash", hash);

            var res = cn.Execute("ActualizarContrasenna", p, commandType: CommandType.StoredProcedure);
            return Ok(res);
        }
    }
}

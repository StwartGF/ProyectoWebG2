using System.Data;
using Dapper;
using ProyectoWebG2.Data;
using ProyectoWebG2.Models;

namespace ProyectoWebG2.Services
{
    public interface IAuthService
    {
        Task<int> RegistrarAsync(RegisterModel vm);                          // >0 IdUsuario; -10 correo; -11 usuario; -12 identificación
        Task<int> IniciarSesionAsync(string usuarioOCorreo, string contrasena);  // >0 IdUsuario; -1 no existe; -2 bloqueado; -3 pass mal
        Task<(int code, string? token, DateTime? expira)> GenerarTokenRecuperacionAsync(string correo);
        Task<int> CambiarContrasenaConTokenAsync(string token, string nuevaContrasena);
        Task<(string NombreCompleto, string Rol)> ObtenerResumenUsuarioAsync(int idUsuario);
        Task<int> SeedAdminAsync(string identificacion, string nombre, string apellidos,
                                 string usuario, string correo, string contrasena,
                                 string rolPorDefecto = "Administrador");
    }

    public class AuthService : IAuthService
    {
        private readonly SqlDb _db;
        public AuthService(SqlDb db) => _db = db;

        public async Task<int> RegistrarAsync(RegisterModel vm)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(vm.Contrasena);

            using var cn = _db.GetConnection();
            var p = new DynamicParameters();
            p.Add("@Identificacion", vm.Identificacion, DbType.String);
            p.Add("@Nombre", vm.Nombre, DbType.String);
            p.Add("@Apellidos", vm.Apellidos, DbType.String);
            p.Add("@Correo", vm.Correo, DbType.String);
            p.Add("@Telefono", vm.Telefono, DbType.String);
            p.Add("@Fecha_Nacimiento", vm.FechaNacimiento, DbType.Date);
            p.Add("@NombreUsuario", vm.Usuario, DbType.String);
            p.Add("@ContrasenaHash", hash, DbType.String);
            p.Add("@RolPorDefecto", "Estudiante", DbType.String);
            p.Add("RETURN_VALUE", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await cn.ExecuteAsync("dbo.USP_Usuario_Registrar", p, commandType: CommandType.StoredProcedure);
            return p.Get<int>("RETURN_VALUE");
        }

        public async Task<int> IniciarSesionAsync(string usuarioOCorreo, string contrasena)
        {
            using var cn = _db.GetConnection();

            // 1) Obtener hash guardado
            const string sqlHash = @"SELECT ContrasenaHash FROM dbo.Usuario WHERE Correo=@u OR NombreUsuario=@u;";
            var hash = await cn.QueryFirstOrDefaultAsync<string>(sqlHash, new { u = usuarioOCorreo });
            if (hash is null) return -1;

            // 2) Verificar contraseña con BCrypt
            if (!BCrypt.Net.BCrypt.Verify(contrasena, hash))
            {
                // Registrar intento fallido en tu SP (pasamos hash incorrecto a propósito)
                var pf = new DynamicParameters();
                pf.Add("@UsuarioOCorreo", usuarioOCorreo);
                pf.Add("@ContrasenaHash", "no_coincide");
                pf.Add("RETURN_VALUE", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await cn.ExecuteAsync("dbo.USP_Usuario_IniciarSesion", pf, commandType: CommandType.StoredProcedure);
                return -3;
            }

            // 3) Éxito → SP resetea intentos y devuelve IdUsuario
            var p = new DynamicParameters();
            p.Add("@UsuarioOCorreo", usuarioOCorreo);
            p.Add("@ContrasenaHash", hash);
            p.Add("RETURN_VALUE", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await cn.ExecuteAsync("dbo.USP_Usuario_IniciarSesion", p, commandType: CommandType.StoredProcedure);
            return p.Get<int>("RETURN_VALUE"); // >0 IdUsuario | -2 bloqueado
        }

        public async Task<(int code, string? token, DateTime? expira)> GenerarTokenRecuperacionAsync(string correo)
        {
            using var cn = _db.GetConnection();
            var p = new DynamicParameters();
            p.Add("@Correo", correo);
            p.Add("@TokenOUT", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
            p.Add("@ExpiraOUT", dbType: DbType.DateTime2, direction: ParameterDirection.Output);
            p.Add("RETURN_VALUE", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await cn.ExecuteAsync("dbo.USP_Usuario_GenerarTokenRecuperacion", p, commandType: CommandType.StoredProcedure);

            return (p.Get<int>("RETURN_VALUE"),
                    p.Get<string>("@TokenOUT"),
                    p.Get<DateTime?>("@ExpiraOUT"));
        }

        public async Task<int> CambiarContrasenaConTokenAsync(string token, string nuevaContrasena)
        {
            using var cn = _db.GetConnection();
            var hash = BCrypt.Net.BCrypt.HashPassword(nuevaContrasena);

            var p = new DynamicParameters();
            p.Add("@Token", token);
            p.Add("@NuevoContrasenaHash", hash);
            p.Add("RETURN_VALUE", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await cn.ExecuteAsync("dbo.USP_Usuario_CambiarContrasena", p, commandType: CommandType.StoredProcedure);
            return p.Get<int>("RETURN_VALUE"); // 1 ok / -1 inválido/expirado
        }

        public async Task<(string NombreCompleto, string Rol)> ObtenerResumenUsuarioAsync(int idUsuario)
        {
            using var cn = _db.GetConnection();
            const string sql = @"
SELECT TOP 1 (u.Nombre + ' ' + u.Apellidos) AS NombreCompleto, r.Nombre AS Rol
FROM dbo.Usuario u
LEFT JOIN dbo.UsuarioRol ur ON ur.IdUsuario = u.IdUsuario
LEFT JOIN dbo.Rol r        ON r.IdRol      = ur.IdRol
WHERE u.IdUsuario = @id
ORDER BY r.IdRol ASC;";
            var data = await cn.QueryFirstOrDefaultAsync<(string, string)>(sql, new { id = idUsuario });
            if (data == default) return ("", "");
            return (data.Item1 ?? "", data.Item2 ?? "");
        }

        public async Task<int> SeedAdminAsync(string identificacion, string nombre, string apellidos,
                                              string usuario, string correo, string contrasena,
                                              string rolPorDefecto = "Administrador")
        {
            using var cn = _db.GetConnection();
            var hash = BCrypt.Net.BCrypt.HashPassword(contrasena);

            var p = new DynamicParameters();
            p.Add("@Identificacion", identificacion);
            p.Add("@Nombre", nombre);
            p.Add("@Apellidos", apellidos);
            p.Add("@Correo", correo);
            p.Add("@Telefono", null);
            p.Add("@Fecha_Nacimiento", null);
            p.Add("@NombreUsuario", usuario);
            p.Add("@ContrasenaHash", hash);
            p.Add("@RolPorDefecto", rolPorDefecto);
            p.Add("RETURN_VALUE", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await cn.ExecuteAsync("dbo.USP_Usuario_Registrar", p, commandType: CommandType.StoredProcedure);
            return p.Get<int>("RETURN_VALUE");
        }
    }
}

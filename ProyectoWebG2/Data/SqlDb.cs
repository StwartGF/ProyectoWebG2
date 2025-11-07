using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ProyectoWebG2.Data
{
    public class SqlDb
    {
        private readonly string _cn;

        public SqlDb(IConfiguration config)
        {
            _cn = config.GetConnectionString("DefaultConnection")
                  ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection");
        }

        public IDbConnection GetConnection() => new SqlConnection(_cn);
    }
}

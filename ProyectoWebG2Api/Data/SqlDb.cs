using System.Data;
using Microsoft.Data.SqlClient;

namespace ProyectoAPI.Data
{
    public class SqlDb
    {
        private readonly string _cn;
        public SqlDb(IConfiguration cfg)
        {
            _cn = cfg.GetConnectionString("BDConnection")
                ?? throw new InvalidOperationException("Falta ConnectionStrings:BDConnection");
        }
        public IDbConnection GetConnection() => new SqlConnection(_cn);
    }
}

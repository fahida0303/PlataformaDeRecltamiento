using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DAL
{
    public abstract class BaseRepository : IDisposable
    {
        protected readonly string _connectionString;

        public BaseRepository()
        {
            // ✅ OPCIÓN 1: Hardcoded (temporal para desarrollo)
            _connectionString = @"Server=LAPTOP-UVS73RFU\SQLEXPRESS;Database=JobsyDB;User Id=usr_ass;Password=psr_ass;Encrypt=False;TrustServerCertificate=True;";
        }

        // Constructor alternativo para inyección de configuración (futuro)
        public BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("JobsyDB");
        }

        protected SqlConnection CrearConexion()
        {
            return new SqlConnection(_connectionString);
        }

        public void Dispose()
        {
            // Recursos adicionales si los hay
        }
    }
}


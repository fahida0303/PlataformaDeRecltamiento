using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public abstract class BaseRepository : IDisposable
    {

        protected readonly string _connectionString;

        public BaseRepository()
        {
            _connectionString = @"Server=DESKTOP-QQR28FH\SQLEXPRESS;Database=JobsDB;Trusted_Connection=True;";
        }

        // Método helper para crear conexiones
        protected SqlConnection CrearConexion()
        {
            return new SqlConnection(_connectionString);
        }

        // Implementación de IDisposable
        public void Dispose()
        {
            // Recursos adicionales si los hay
        }
    }
}


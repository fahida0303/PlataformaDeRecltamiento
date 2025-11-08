using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AdministradorRepository : BaseRepository, IRepository<Administrador>
    {
        public Response<Administrador> Insertar(Administrador entidad)
        {
            try
            {
                if (entidad == null || string.IsNullOrWhiteSpace(entidad.Nombre))
                {
                    return new Response<Administrador>(false, "El nombre es requerido", null, null);
                }

                string sentencia = @"INSERT INTO [dbo.Administrador] ([permiso, [idUsuario]) 
                                     VALUES (@permiso, @idUsuario)";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@permiso", entidad.Permisos ?? "");
                    comando.Parameters.AddWithValue("@idUsuario", entidad.IdUsuario);

                    conexion.Open();
                    comando.ExecuteNonQuery();

                    return new Response<Administrador>(true, "Administrador insertado correctamente", entidad, null);
                }
            }
            catch (SqlException ex)
            {
                return new Response<Administrador>(false, $"Error en BD: {ex.Message}", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Administrador> Actualizar(Administrador entidad)
        {
            try
            {
                if (entidad == null || entidad.IdUsuario <= 0)
                {
                    return new Response<Administrador>(false, "Datos inválidos", null, null);
                }

                string sentencia = @"UPDATE [dbo.Administrador] SET [permiso] = @permiso 
                                     WHERE [idUsuario] = @idUsuario";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@permiso", entidad.Permisos ?? "");
                    comando.Parameters.AddWithValue("@idUsuario", entidad.IdUsuario);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<Administrador>(true, "Actualizado correctamente", entidad, null);
                    }
                    return new Response<Administrador>(false, "No se encontró", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Administrador> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Administrador>(false, "ID inválido", null, null);

                string sentencia = "DELETE FROM [dbo.Administrador] WHERE [idUsuario] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Administrador>(true, "Eliminado correctamente", null, null);
                    return new Response<Administrador>(false, "No encontrado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Administrador> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Administrador>(false, "ID inválido", null, null);

                string sentencia = @"SELECT u.[idUsuario], u.[nombre], u.[correo], u.[contrasena], u.[estado], a.[permiso]
                                     FROM [dbo.Usuario] u
                                     INNER JOIN [dbo.Administrador] a ON u.[idUsuario] = a.[idUsuario]
                                     WHERE u.[idUsuario] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Administrador admin = new Administrador
                            {
                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Correo = reader.GetString(2),
                                Contrasena = reader.GetString(3),
                                Estado = reader.GetString(4),
                                Permisos = reader.GetString(5)
                            };
                            return new Response<Administrador>(true, "Encontrado", admin, null);
                        }
                        return new Response<Administrador>(false, "No encontrado", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Administrador> ObtenerTodos()
        {
            try
            {
                IList<Administrador> lista = new List<Administrador>();
                string sentencia = @"SELECT u.[idUsuario], u.[nombre], u.[correo], u.[contrasena], u.[estado], a.[permiso]
                                     FROM [dbo.Usuario] u
                                     INNER JOIN [dbo.Administrador] a ON u.[idUsuario] = a.[idUsuario]
                                     ORDER BY u.[idUsuario]";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Administrador
                            {
                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Correo = reader.GetString(2),
                                Contrasena = reader.GetString(3),
                                Estado = reader.GetString(4),
                                Permisos = reader.GetString(5)
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Administrador>(true, $"Se encontraron {lista.Count}", null, lista);
                return new Response<Administrador>(true, "Sin registros", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL
{
    public class AdministradorRepository : BaseRepository, IRepository<Administrador>
    {
        public Response<Administrador> Insertar(Administrador entidad)
        {
            try
            {
                if (entidad == null || string.IsNullOrWhiteSpace(entidad.Permisos))
                {
                    return new Response<Administrador>(false, "Los permisos son requeridos", null, null);
                }

                string sentencia = @"INSERT INTO [Administrador] ([permisos], [idUsuario]) 
                                 VALUES (@permisos, @idUsuario)";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@permisos", entidad.Permisos ?? "");
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
                if (entidad == null || entidad.IdAdmin <= 0)
                {
                    return new Response<Administrador>(false, "Datos inválidos", null, null);
                }

                string sentencia = @"UPDATE [Administrador] SET [permisos] = @permisos 
                                 WHERE [idAdmin] = @idAdmin";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@permisos", entidad.Permisos ?? "");
                    comando.Parameters.AddWithValue("@idAdmin", entidad.IdAdmin);

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

                string sentencia = "DELETE FROM [Administrador] WHERE [idAdmin] = @id";

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

                string sentencia = @"SELECT a.[idAdmin], a.[idUsuario], u.[nombre], u.[correo], u.[contraseña], u.[estado], a.[permisos]
                                 FROM [Administrador] a
                                 INNER JOIN [Usuario] u ON a.[idUsuario] = u.[idUsuario]
                                 WHERE a.[idAdmin] = @id";

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
                                IdAdmin = reader.GetInt32(0),      // ✅ CORRECCIÓN: Agregado
                                IdUsuario = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Correo = reader.GetString(3),
                                Contrasena = reader.GetString(4),
                                Estado = reader.GetString(5),
                                Permisos = reader.GetString(6)
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
                string sentencia = @"SELECT a.[idAdmin], a.[idUsuario], u.[nombre], u.[correo], u.[contraseña], u.[estado], a.[permisos]
                                 FROM [Administrador] a
                                 INNER JOIN [Usuario] u ON a.[idUsuario] = u.[idUsuario]
                                 ORDER BY a.[idAdmin]";

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
                                IdAdmin = reader.GetInt32(0),    // ✅ CORRECCIÓN: Agregado
                                IdUsuario = reader.GetInt32(1),
                                Nombre = reader.GetString(2),
                                Correo = reader.GetString(3),
                                Contrasena = reader.GetString(4),
                                Estado = reader.GetString(5),
                                Permisos = reader.GetString(6)
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
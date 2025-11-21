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

                const string sentencia = @"
                    INSERT INTO [Administrador] ([idUsuario], [permisos])
                    VALUES (@idUsuario, @permisos);
                    SELECT SCOPE_IDENTITY();";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idUsuario", entidad.IdUsuario);
                    comando.Parameters.AddWithValue("@permisos", entidad.Permisos ?? (object)DBNull.Value);

                    conexion.Open();
                    var result = comando.ExecuteScalar();
                    entidad.IdAdmin = Convert.ToInt32(result);

                    return new Response<Administrador>(true, "Administrador creado correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error al insertar administrador: {ex.Message}", null, null);
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

                const string sentencia = @"
                    UPDATE [Administrador]
                    SET [permisos] = @permisos
                    WHERE [idAdmin] = @idAdmin;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@permisos", entidad.Permisos ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@idAdmin", entidad.IdAdmin);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<Administrador>(true, "Administrador actualizado correctamente", entidad, null);

                    return new Response<Administrador>(false, "No se encontró el administrador a actualizar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error al actualizar: {ex.Message}", null, null);
            }
        }

        public Response<Administrador> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Administrador>(false, "Id inválido", null, null);

                const string sentencia = @"DELETE FROM [Administrador] WHERE [idAdmin] = @idAdmin;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idAdmin", id);
                    conexion.Open();

                    int filas = comando.ExecuteNonQuery();
                    if (filas > 0)
                        return new Response<Administrador>(true, "Administrador eliminado correctamente", null, null);

                    return new Response<Administrador>(false, "No se encontró el administrador a eliminar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error al eliminar: {ex.Message}", null, null);
            }
        }

        public Response<Administrador> ObtenerPorId(int id)
        {
            try
            {
                const string sentencia = @"
                    SELECT a.idAdmin, a.idUsuario, a.permisos,
                           u.nombre, u.correo
                    FROM Administrador a
                    INNER JOIN Usuario u ON a.idUsuario = u.idUsuario
                    WHERE a.idAdmin = @idAdmin;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idAdmin", id);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var admin = new Administrador
                            {
                                IdAdmin = reader.GetInt32(reader.GetOrdinal("idAdmin")),
                                IdUsuario = reader.GetInt32(reader.GetOrdinal("idUsuario")),
                                Permisos = reader["permisos"] as string,
                                Nombre = reader["nombre"] as string,
                                Correo = reader["correo"] as string
                            };

                            return new Response<Administrador>(true, "Administrador encontrado", admin, null);
                        }
                    }
                }

                return new Response<Administrador>(false, "No se encontró el administrador", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error al obtener por id: {ex.Message}", null, null);
            }
        }

        public Response<Administrador> ObtenerTodos()
        {
            try
            {
                var lista = new List<Administrador>();

                const string sentencia = @"
                    SELECT a.idAdmin, a.idUsuario, a.permisos,
                           u.nombre, u.correo
                    FROM Administrador a
                    INNER JOIN Usuario u ON a.idUsuario = u.idUsuario;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var admin = new Administrador
                            {
                                IdAdmin = reader.GetInt32(reader.GetOrdinal("idAdmin")),
                                IdUsuario = reader.GetInt32(reader.GetOrdinal("idUsuario")),
                                Permisos = reader["permisos"] as string,
                                Nombre = reader["nombre"] as string,
                                Correo = reader["correo"] as string
                            };

                            lista.Add(admin);
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Administrador>(true, $"Se encontraron {lista.Count} administradores", null, lista);

                return new Response<Administrador>(true, "No hay administradores registrados", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Administrador>(false, $"Error al obtener todos: {ex.Message}", null, null);
            }
        }
    }
}

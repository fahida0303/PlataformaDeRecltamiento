using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UsuarioRepository : BaseRepository, IRepository<Usuario>
    {
        public Response<Usuario> Insertar(Usuario entidad)
        {
            try
            {
                if (entidad == null || string.IsNullOrWhiteSpace(entidad.Nombre) ||
                    string.IsNullOrWhiteSpace(entidad.Correo) || string.IsNullOrWhiteSpace(entidad.Contrasena))
                {
                    return new Response<Usuario>(false, "Los campos Nombre, Correo y Contraseña son requeridos", null, null);
                }

                string sentencia = "INSERT INTO [Usuarios] ([Nombre], [Correo], [Contrasena], [Estado]) VALUES (@nombre, @correo, @contrasena, @estado); SELECT SCOPE_IDENTITY();";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@correo", entidad.Correo);
                    comando.Parameters.AddWithValue("@contrasena", entidad.Contrasena);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Activo");

                    conexion.Open();
                    int nuevoId = Convert.ToInt32(comando.ExecuteScalar());
                    entidad.IdUsuario = nuevoId;

                    return new Response<Usuario>(true, "Usuario insertado correctamente", entidad, null);
                }
            }
            catch (SqlException ex)
            {
                return new Response<Usuario>(false, $"Error en la base de datos: \n {ex.Message} - SQL_ERROR", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(false, $"Error al insertar el usuario \n {ex.Message}", null, null);
            }
        }

        public Response<Usuario> Actualizar(Usuario entidad)
        {
            try
            {
                if (entidad == null || entidad.IdUsuario <= 0)
                {
                    return new Response<Usuario>(false, "Datos inválidos para actualizar", null, null);
                }

                string sentencia = "UPDATE [Usuarios] SET [Nombre] = @nombre, [Correo] = @correo, [Contrasena] = @contrasena, [Estado] = @estado WHERE [IdUsuario] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre ?? "");
                    comando.Parameters.AddWithValue("@correo", entidad.Correo ?? "");
                    comando.Parameters.AddWithValue("@contrasena", entidad.Contrasena ?? "");
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Activo");
                    comando.Parameters.AddWithValue("@id", entidad.IdUsuario);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<Usuario>(true, "Usuario actualizado correctamente", entidad, null);
                    }
                    else
                    {
                        return new Response<Usuario>(false, "No se encontró el usuario para actualizar", null, null);
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<Usuario>(false, $"Error en la base de datos: \n {ex.Message} - SQL_ERROR", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(false, $"Error al actualizar el usuario \n {ex.Message}", null, null);
            }
        }

        public Response<Usuario> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Usuario>(false, "El ID debe ser mayor a cero", null, null);
                }

                string sentencia = "DELETE FROM [Usuarios] WHERE [IdUsuario] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<Usuario>(true, "Usuario eliminado correctamente", null, null);
                    }
                    else
                    {
                        return new Response<Usuario>(false, "No se encontró el usuario con el ID especificado", null, null);
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    return new Response<Usuario>(false, "No se puede eliminar: el usuario está siendo utilizado", null, null);
                }
                return new Response<Usuario>(false, $"Error en la base de datos: \n {ex.Message} - SQL_ERROR", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(false, $"Error al eliminar el usuario \n {ex.Message}", null, null);
            }
        }

        public Response<Usuario> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Usuario>(false, "El ID debe ser mayor a cero", null, null);
                }

                string sentencia = "SELECT [IdUsuario], [Nombre], [Correo], [Contrasena], [Estado] FROM [Usuarios] WHERE [IdUsuario] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Usuario usuario = MapearUsuario(reader);
                            return new Response<Usuario>(true, "Usuario encontrado", usuario, null);
                        }
                        else
                        {
                            return new Response<Usuario>(false, "No se encontró el usuario con el ID especificado", null, null);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<Usuario>(false, $"Error en la base de datos: \n {ex.Message} - SQL_ERROR", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(false, $"Error al obtener el usuario \n {ex.Message}", null, null);
            }
        }

        public Response<Usuario> ObtenerTodos()
        {
            try
            {
                IList<Usuario> listaUsuarios = new List<Usuario>();
                string sentencia = "SELECT [IdUsuario], [Nombre], [Correo], [Contrasena], [Estado] FROM [Usuarios] ORDER BY [Nombre]";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaUsuarios.Add(MapearUsuario(reader));
                        }
                    }
                }

                if (listaUsuarios.Count > 0)
                {
                    return new Response<Usuario>(true, $"Se encontraron {listaUsuarios.Count} usuarios", null, listaUsuarios);
                }
                else
                {
                    return new Response<Usuario>(true, "No hay usuarios registrados", null, listaUsuarios);
                }
            }
            catch (SqlException ex)
            {
                return new Response<Usuario>(false, $"Error en la base de datos: \n {ex.Message} - SQL_ERROR", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(false, $"Error al obtener los usuarios \n {ex.Message}", null, null);
            }
        }

        private Usuario MapearUsuario(SqlDataReader reader)
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Correo = reader.GetString(2),
                Contrasena = reader.GetString(3),
                Estado = reader.GetString(4)
            };
        }
    }
}

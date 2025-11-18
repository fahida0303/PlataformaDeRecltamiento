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
        // INSERTAR
        public Response<Usuario> Insertar(Usuario entidad)
        {
            try
            {
                if (entidad == null ||
                    string.IsNullOrWhiteSpace(entidad.Nombre) ||
                    string.IsNullOrWhiteSpace(entidad.Correo) ||
                    string.IsNullOrWhiteSpace(entidad.Contrasena))
                {
                    return new Response<Usuario>(
                        false,
                        "Los campos Nombre, Correo y Contraseña son requeridos",
                        null,
                        null
                    );
                }

                string sentencia = @"
                    INSERT INTO [Usuario] 
                    ([nombre], [correo], [contraseña], [estado], 
                     [telegramId], [telegramUsername], [whatsappNumber],
                     [tipoUsuario], [documento], [fechaNacimiento], [foto]) 
                    VALUES (@nombre, @correo, @contrasena, @estado, 
                            @telegramId, @telegramUsername, @whatsappNumber,
                            @tipoUsuario, @documento, @fechaNacimiento, @foto); 
                    SELECT SCOPE_IDENTITY();";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre ?? "");
                    comando.Parameters.AddWithValue("@correo", entidad.Correo ?? "");
                    comando.Parameters.AddWithValue("@contrasena", entidad.Contrasena ?? "");
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Activo");

                    comando.Parameters.AddWithValue("@telegramId", (object)entidad.TelegramId ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@telegramUsername", (object)entidad.TelegramUsername ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@whatsappNumber", (object)entidad.WhatsappNumber ?? DBNull.Value);

                    comando.Parameters.AddWithValue("@tipoUsuario", (object)entidad.TipoUsuario ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@documento", (object)entidad.Documento ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@fechaNacimiento", (object)entidad.FechaNacimiento ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@foto", (object)entidad.Foto ?? DBNull.Value);

                    conexion.Open();
                    int nuevoId = Convert.ToInt32(comando.ExecuteScalar());
                    entidad.IdUsuario = nuevoId;

                    return new Response<Usuario>(
                        true,
                        "Usuario insertado correctamente",
                        entidad,
                        null
                    );
                }
            }
            catch (SqlException ex)
            {
                return new Response<Usuario>(
                    false,
                    $"Error en la base de datos: \n{ex.Message} - SQL_ERROR",
                    null,
                    null
                );
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(
                    false,
                    $"Error al insertar el usuario \n{ex.Message}",
                    null,
                    null
                );
            }
        }

        // ACTUALIZAR
        public Response<Usuario> Actualizar(Usuario entidad)
        {
            try
            {
                if (entidad == null || entidad.IdUsuario <= 0)
                {
                    return new Response<Usuario>(
                        false,
                        "Datos inválidos para actualizar",
                        null,
                        null
                    );
                }

                string sentencia = @"
                    UPDATE [Usuario] 
                    SET [nombre] = @nombre, 
                        [correo] = @correo, 
                        [contraseña] = @contrasena, 
                        [estado] = @estado,
                        [telegramId] = @telegramId,
                        [telegramUsername] = @telegramUsername,
                        [whatsappNumber] = @whatsappNumber,
                        [tipoUsuario] = @tipoUsuario,
                        [documento] = @documento,
                        [fechaNacimiento] = @fechaNacimiento,
                        [foto] = @foto
                    WHERE [idUsuario] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre ?? "");
                    comando.Parameters.AddWithValue("@correo", entidad.Correo ?? "");
                    comando.Parameters.AddWithValue("@contrasena", entidad.Contrasena ?? "");
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Activo");

                    comando.Parameters.AddWithValue("@telegramId", (object)entidad.TelegramId ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@telegramUsername", (object)entidad.TelegramUsername ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@whatsappNumber", (object)entidad.WhatsappNumber ?? DBNull.Value);

                    comando.Parameters.AddWithValue("@tipoUsuario", (object)entidad.TipoUsuario ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@documento", (object)entidad.Documento ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@fechaNacimiento", (object)entidad.FechaNacimiento ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@foto", (object)entidad.Foto ?? DBNull.Value);

                    comando.Parameters.AddWithValue("@id", entidad.IdUsuario);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<Usuario>(
                            true,
                            "Usuario actualizado correctamente",
                            entidad,
                            null
                        );
                    }
                    else
                    {
                        return new Response<Usuario>(
                            false,
                            "No se encontró el usuario para actualizar",
                            null,
                            null
                        );
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<Usuario>(
                    false,
                    $"Error en la base de datos: \n{ex.Message} - SQL_ERROR",
                    null,
                    null
                );
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(
                    false,
                    $"Error al actualizar el usuario \n{ex.Message}",
                    null,
                    null
                );
            }
        }

        // ELIMINAR (borrado lógico)
        public Response<Usuario> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Usuario>(
                        false,
                        "El ID debe ser mayor a cero",
                        null,
                        null
                    );
                }

                // Borrado lógico: estado = 'Inactivo'
                string sentencia = @"
                    UPDATE [Usuario] 
                    SET [estado] = 'Inactivo' 
                    WHERE [idUsuario] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<Usuario>(
                            true,
                            "Usuario desactivado correctamente",
                            null,
                            null
                        );
                    }
                    else
                    {
                        return new Response<Usuario>(
                            false,
                            "No se encontró el usuario con el ID especificado",
                            null,
                            null
                        );
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<Usuario>(
                    false,
                    $"Error en la base de datos: \n{ex.Message} - SQL_ERROR",
                    null,
                    null
                );
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(
                    false,
                    $"Error al desactivar el usuario \n{ex.Message}",
                    null,
                    null
                );
            }
        }

        // OBTENER POR ID
        public Response<Usuario> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Usuario>(
                        false,
                        "El ID debe ser mayor a cero",
                        null,
                        null
                    );
                }

                string sentencia = @"
                    SELECT [idUsuario], [nombre], [correo], 
                           [contraseña], [estado], [telegramId], 
                           [telegramUsername], [whatsappNumber], 
                           [fechaUltimaInteraccionBot],
                           [tipoUsuario], [documento], [fechaNacimiento], [foto]
                    FROM [Usuario] 
                    WHERE [idUsuario] = @id";

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
                            return new Response<Usuario>(
                                true,
                                "Usuario encontrado",
                                usuario,
                                null
                            );
                        }
                        else
                        {
                            return new Response<Usuario>(
                                false,
                                "No se encontró el usuario con el ID especificado",
                                null,
                                null
                            );
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<Usuario>(
                    false,
                    $"Error en la base de datos: \n{ex.Message} - SQL_ERROR",
                    null,
                    null
                );
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(
                    false,
                    $"Error al obtener el usuario \n{ex.Message}",
                    null,
                    null
                );
            }
        }

        // OBTENER TODOS
        public Response<Usuario> ObtenerTodos()
        {
            try
            {
                IList<Usuario> listaUsuarios = new List<Usuario>();

                string sentencia = @"
                    SELECT [idUsuario], [nombre], [correo], 
                           [contraseña], [estado], [telegramId], 
                           [telegramUsername], [whatsappNumber], 
                           [fechaUltimaInteraccionBot],
                           [tipoUsuario], [documento], [fechaNacimiento], [foto]
                    FROM [Usuario] 
                    ORDER BY [nombre]";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Usuario usuario = MapearUsuario(reader);
                            listaUsuarios.Add(usuario);
                        }
                    }
                }

                return new Response<Usuario>(
                    true,
                    "Usuarios obtenidos correctamente",
                    null,
                    listaUsuarios
                );
            }
            catch (SqlException ex)
            {
                return new Response<Usuario>(
                    false,
                    $"Error en la base de datos: \n{ex.Message} - SQL_ERROR",
                    null,
                    null
                );
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(
                    false,
                    $"Error al obtener los usuarios \n{ex.Message}",
                    null,
                    null
                );
            }
        }

        // OPCIONAL: Obtener por TelegramId (para bots)
        public Response<Usuario> ObtenerPorTelegramId(string telegramId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(telegramId))
                {
                    return new Response<Usuario>(
                        false,
                        "TelegramId no puede estar vacío",
                        null,
                        null
                    );
                }

                string sentencia = @"
                    SELECT [idUsuario], [nombre], [correo], 
                           [contraseña], [estado], [telegramId], 
                           [telegramUsername], [whatsappNumber], 
                           [fechaUltimaInteraccionBot],
                           [tipoUsuario], [documento], [fechaNacimiento], [foto]
                    FROM [Usuario] 
                    WHERE [telegramId] = @telegramId";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@telegramId", telegramId);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Usuario usuario = MapearUsuario(reader);
                            return new Response<Usuario>(
                                true,
                                "Usuario encontrado por TelegramId",
                                usuario,
                                null
                            );
                        }
                        else
                        {
                            return new Response<Usuario>(
                                false,
                                "No se encontró usuario con ese TelegramId",
                                null,
                                null
                            );
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<Usuario>(
                    false,
                    $"Error en la base de datos: \n{ex.Message} - SQL_ERROR",
                    null,
                    null
                );
            }
            catch (Exception ex)
            {
                return new Response<Usuario>(
                    false,
                    $"Error al obtener usuario por TelegramId \n{ex.Message}",
                    null,
                    null
                );
            }
        }

        // MAPEADOR
        private Usuario MapearUsuario(SqlDataReader reader)
        {
            // El orden de índices debe coincidir con los SELECT
            return new Usuario
            {
                IdUsuario = reader.GetInt32(0),                      // idUsuario
                Nombre = reader.GetString(1),                       // nombre
                Correo = reader.GetString(2),                       // correo
                Contrasena = reader.GetString(3),                   // contraseña
                Estado = reader.GetString(4),                       // estado
                TelegramId = reader.IsDBNull(5) ? null : reader.GetString(5),
                TelegramUsername = reader.IsDBNull(6) ? null : reader.GetString(6),
                WhatsappNumber = reader.IsDBNull(7) ? null : reader.GetString(7),
                FechaUltimaInteraccionBot = reader.IsDBNull(8)
                    ? (DateTime?)null
                    : reader.GetDateTime(8),
                TipoUsuario = reader.IsDBNull(9)
                    ? null
                    : reader.GetString(9),
                Documento = reader.IsDBNull(10)
                    ? null
                    : reader.GetString(10),
                FechaNacimiento = reader.IsDBNull(11)
                    ? (DateTime?)null
                    : reader.GetDateTime(11),
                Foto = reader.IsDBNull(12)
                    ? null
                    : (byte[])reader["foto"]
            };
        }
    }
}


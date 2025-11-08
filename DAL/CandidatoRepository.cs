using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class CandidatoRepository : BaseRepository, IRepository<Candidato>
    {
        public Response<Candidato> Insertar(Candidato entidad)
        {
            try
            {
                if (entidad == null || string.IsNullOrWhiteSpace(entidad.Nombre))
                {
                    return new Response<Candidato>(false, "El nombre es requerido", null, null);
                }

                string sentencia = @"INSERT INTO [dbo.Candidato] ([nivelFormacion], [experiencia], [hojaDeVida], [idUsuario]) 
                                     VALUES (@nivelFormacion, @experiencia, @hojaDeVida, @idUsuario)";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nivelFormacion", entidad.NivelFormacion ?? "");
                    comando.Parameters.AddWithValue("@experiencia", entidad.Experiencia ?? "");
                    comando.Parameters.AddWithValue("@hojaDeVida", entidad.HojaDeVida ?? "");
                    comando.Parameters.AddWithValue("@idUsuario", entidad.IdUsuario);

                    conexion.Open();
                    comando.ExecuteNonQuery();

                    return new Response<Candidato>(true, "Candidato insertado correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Candidato> Actualizar(Candidato entidad)
        {
            try
            {
                if (entidad == null || entidad.IdUsuario <= 0)
                {
                    return new Response<Candidato>(false, "Datos inválidos", null, null);
                }

                string sentencia = @"UPDATE [dbo.Candidato] SET [nivelFormacion] = @nivelFormacion,
                                     [experiencia] = @experiencia, [hojaDeVida] = @hojaDeVida
                                     WHERE [idUsuario] = @idUsuario";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nivelFormacion", entidad.NivelFormacion ?? "");
                    comando.Parameters.AddWithValue("@experiencia", entidad.Experiencia ?? "");
                    comando.Parameters.AddWithValue("@hojaDeVida", entidad.HojaDeVida ?? "");
                    comando.Parameters.AddWithValue("@idUsuario", entidad.IdUsuario);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Candidato>(true, "Actualizado", entidad, null);
                    return new Response<Candidato>(false, "No encontrado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Candidato> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Candidato>(false, "ID inválido", null, null);

                string sentencia = "DELETE FROM [dbo.Candidato] WHERE [idUsuario] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Candidato>(true, "Eliminado", null, null);
                    return new Response<Candidato>(false, "No encontrado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Candidato> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Candidato>(false, "ID inválido", null, null);

                string sentencia = @"SELECT u.[idUsuario], u.[nombre], u.[correo], u.[contrasena], u.[estado], 
                                     c.[nivelFormacion], c.[experiencia], c.[hojaDeVida]
                                     FROM [dbo.Usuario] u
                                     INNER JOIN [dbo.Candidato] c ON u.[idUsuario] = c.[idUsuario]
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
                            Candidato candidato = new Candidato
                            {
                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Correo = reader.GetString(2),
                                Contrasena = reader.GetString(3),
                                Estado = reader.GetString(4),
                                NivelFormacion = reader.GetString(5),
                                Experiencia = reader.GetString(6),
                                HojaDeVida = reader.GetString(7)
                            };
                            return new Response<Candidato>(true, "Encontrado", candidato, null);
                        }
                        return new Response<Candidato>(false, "No encontrado", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Candidato> ObtenerTodos()
        {
            try
            {
                IList<Candidato> lista = new List<Candidato>();
                string sentencia = @"SELECT u.[idUsuario], u.[nombre], u.[correo], u.[contrasena], u.[estado],
                                     c.[nivelFormacion], c.[experiencia], c.[hojaDeVida]
                                     FROM [dbo.Usuario] u
                                     INNER JOIN [dbo.Candidato] c ON u.[idUsuario] = c.[idUsuario]
                                     ORDER BY u.[nombre]";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Candidato
                            {
                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Correo = reader.GetString(2),
                                Contrasena = reader.GetString(3),
                                Estado = reader.GetString(4),
                                NivelFormacion = reader.GetString(5),
                                Experiencia = reader.GetString(6),
                                HojaDeVida = reader.GetString(7)
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Candidato>(true, $"Se encontraron {lista.Count}", null, lista);
                return new Response<Candidato>(true, "Sin registros", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

using System;
using ENTITY;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PostulacionRepository : BaseRepository, IRepository<Postulacion>
    {
        public Response<Postulacion> Insertar(Postulacion entidad)
        {
            try
            {
                if (entidad == null || entidad.IdCandidato <= 0 || entidad.IdConvocatoria <= 0)
                {
                    return new Response<Postulacion>(false, "Los datos de la postulación son inválidos", null, null);
                }

                string sentencia = "INSERT INTO [Postulaciones] ([IdCandidato], [IdConvocatoria], [FechaPostulacion], [Estado]) VALUES (@idCandidato, @idConvocatoria, @fechaPostulacion, @estado); SELECT SCOPE_IDENTITY();";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idConvocatoria", entidad.IdConvocatoria);
                    comando.Parameters.AddWithValue("@fechaPostulacion", entidad.FechaPostulacion);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Pendiente");

                    conexion.Open();
                    int nuevoId = Convert.ToInt32(comando.ExecuteScalar());
                    entidad.IdPostulacion = nuevoId;

                    return new Response<Postulacion>(true, "Postulación insertada correctamente", entidad, null);
                }
            }
            catch (SqlException ex)
            {
                return new Response<Postulacion>(false, $"Error en la base de datos: \n {ex.Message} - SQL_ERROR", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al insertar la postulación \n {ex.Message}", null, null);
            }
        }

        public Response<Postulacion> Actualizar(Postulacion entidad)
        {
            try
            {
                if (entidad == null || entidad.IdPostulacion <= 0)
                {
                    return new Response<Postulacion>(false, "Datos inválidos para actualizar", null, null);
                }

                string sentencia = "UPDATE [Postulaciones] SET [IdCandidato] = @idCandidato, [IdConvocatoria] = @idConvocatoria, [FechaPostulacion] = @fechaPostulacion, [Estado] = @estado WHERE [IdPostulacion] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idConvocatoria", entidad.IdConvocatoria);
                    comando.Parameters.AddWithValue("@fechaPostulacion", entidad.FechaPostulacion);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Pendiente");
                    comando.Parameters.AddWithValue("@id", entidad.IdPostulacion);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<Postulacion>(true, "Postulación actualizada correctamente", entidad, null);
                    }
                    else
                    {
                        return new Response<Postulacion>(false, "No se encontró la postulación para actualizar", null, null);
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<Postulacion>(false, $"Error en la base de datos: \n {ex.Message} - SQL_ERROR", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al actualizar la postulación \n {ex.Message}", null, null);
            }
        }

        public Response<Postulacion> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Postulacion>(false, "El ID debe ser mayor a cero", null, null);
                }

                string sentencia = "DELETE FROM [Postulaciones] WHERE [IdPostulacion] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<Postulacion>(true, "Postulación eliminada correctamente", null, null);
                    }
                    else
                    {
                        return new Response<Postulacion>(false, "No se encontró la postulación con el ID especificado", null, null);
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    return new Response<Postulacion>(false, "No se puede eliminar: la postulación está siendo utilizada", null, null);
                }
                return new Response<Postulacion>(false, $"Error en la base de datos: \n {ex.Message} - SQL_ERROR", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al eliminar la postulación \n {ex.Message}", null, null);
            }
        }

        public Response<Postulacion> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Postulacion>(false, "El ID debe ser mayor a cero", null, null);
                }

                string sentencia = "SELECT [IdPostulacion], [IdCandidato], [IdConvocatoria], [FechaPostulacion], [Estado] FROM [Postulaciones] WHERE [IdPostulacion] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Postulacion postulacion = MapearPostulacion(reader);
                            return new Response<Postulacion>(true, "Postulación encontrada", postulacion, null);
                        }
                        else
                        {
                            return new Response<Postulacion>(false, "No se encontró la postulación con el ID especificado", null, null);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<Postulacion>(false, $"Error en la base de datos: \n {ex.Message} - SQL_ERROR", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al obtener la postulación \n {ex.Message}", null, null);
            }
        }

        public Response<Postulacion> ObtenerTodos()
        {
            try
            {
                IList<Postulacion> listaPostulaciones = new List<Postulacion>();
                string sentencia = "SELECT [IdPostulacion], [IdCandidato], [IdConvocatoria], [FechaPostulacion], [Estado] FROM [Postulaciones] ORDER BY [FechaPostulacion] DESC";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaPostulaciones.Add(MapearPostulacion(reader));
                        }
                    }
                }

                if (listaPostulaciones.Count > 0)
                {
                    return new Response<Postulacion>(true, $"Se encontraron {listaPostulaciones.Count} postulaciones", null, listaPostulaciones);
                }
                else
                {
                    return new Response<Postulacion>(true, "No hay postulaciones registradas", null, listaPostulaciones);
                }
            }
            catch (SqlException ex)
            {
                return new Response<Postulacion>(false, $"Error en la base de datos: \n {ex.Message} - SQL_ERROR", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al obtener las postulaciones \n {ex.Message}", null, null);
            }
        }

        private Postulacion MapearPostulacion(SqlDataReader reader)
        {
            return new Postulacion
            {
                IdPostulacion = reader.GetInt32(0),
                IdCandidato = reader.GetInt32(1),
                IdConvocatoria = reader.GetInt32(2),
                FechaPostulacion = reader.GetDateTime(3),
                Estado = reader.GetString(4)
            };
        }
    }
}

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

                string sentencia = @"INSERT INTO [Postulacion] ([fecha_postulacion], [estado], [idCandidato], [idConvocatoria]) 
                                 VALUES (@fechaPostulacion, @estado, @idCandidato, @idConvocatoria)";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@fechaPostulacion", entidad.FechaPostulacion);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "En revisión");
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idConvocatoria", entidad.IdConvocatoria);

                    conexion.Open();
                    comando.ExecuteNonQuery();

                    return new Response<Postulacion>(true, "Postulación insertada correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error: {ex.Message}", null, null);
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

                string sentencia = @"UPDATE [Postulacion] SET [fecha_postulacion] = @fechaPostulacion, [estado] = @estado, 
                                 [idCandidato] = @idCandidato, [idConvocatoria] = @idConvocatoria 
                                 WHERE [idPostulacion] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@fechaPostulacion", entidad.FechaPostulacion);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "En revisión");
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idConvocatoria", entidad.IdConvocatoria);
                    comando.Parameters.AddWithValue("@id", entidad.IdPostulacion);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Postulacion>(true, "Postulación actualizada correctamente", entidad, null);
                    return new Response<Postulacion>(false, "No se encontró la postulación para actualizar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Postulacion> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Postulacion>(false, "El ID debe ser mayor a cero", null, null);

                string sentencia = "DELETE FROM [Postulacion] WHERE [idPostulacion] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Postulacion>(true, "Postulación eliminada correctamente", null, null);
                    return new Response<Postulacion>(false, "No se encontró la postulación con el ID especificado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Postulacion> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Postulacion>(false, "El ID debe ser mayor a cero", null, null);

                string sentencia = @"SELECT [idPostulacion], [fecha_postulacion], [estado], [idCandidato], [idConvocatoria] 
                                 FROM [Postulacion] WHERE [idPostulacion] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Postulacion postulacion = new Postulacion
                            {
                                IdPostulacion = reader.GetInt32(0),
                                FechaPostulacion = reader.GetDateTime(1),
                                Estado = reader.GetString(2),
                                IdCandidato = reader.GetInt32(3),
                                IdConvocatoria = reader.GetInt32(4)
                            };
                            return new Response<Postulacion>(true, "Postulación encontrada", postulacion, null);
                        }
                        return new Response<Postulacion>(false, "No se encontró la postulación con el ID especificado", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Postulacion> ObtenerTodos()
        {
            try
            {
                IList<Postulacion> lista = new List<Postulacion>();
                string sentencia = @"SELECT [idPostulacion], [fecha_postulacion], [estado], [idCandidato], [idConvocatoria] 
                                 FROM [Postulacion] ORDER BY [fecha_postulacion] DESC";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Postulacion
                            {
                                IdPostulacion = reader.GetInt32(0),
                                FechaPostulacion = reader.GetDateTime(1),
                                Estado = reader.GetString(2),
                                IdCandidato = reader.GetInt32(3),
                                IdConvocatoria = reader.GetInt32(4)
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Postulacion>(true, $"Se encontraron {lista.Count}", null, lista);
                return new Response<Postulacion>(true, "No hay postulaciones registradas", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

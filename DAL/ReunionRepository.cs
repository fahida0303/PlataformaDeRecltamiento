using System;
using ENTITY;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ReunionRepository : BaseRepository, IRepository<Reunion>
    {
        public Response<Reunion> Insertar(Reunion entidad)
        {
            try
            {
                if (entidad == null)
                    return new Response<Reunion>(false, "La entidad no puede ser nula", null, null);

                if (entidad.IdCandidato <= 0 || entidad.IdReclutador <= 0)
                    return new Response<Reunion>(false, "Debe especificar IdCandidato e IdReclutador válidos", null, null);

                string sentencia = @"INSERT INTO [Reunion] ([fecha], [enlaceMeet], [estadoConfirmacion], [idCandidato], [idReclutador]) 
                                 VALUES (@fecha, @enlaceMeet, @estadoConfirmacion, @idCandidato, @idReclutador);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@fecha", entidad.Fecha);
                    comando.Parameters.AddWithValue("@enlaceMeet", entidad.EnlaceMeet ?? "");
                    comando.Parameters.AddWithValue("@estadoConfirmacion", entidad.EstadoConfirmacion ?? "Pendiente");
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idReclutador", entidad.IdReclutador);

                    conexion.Open();
                    int nuevoId = Convert.ToInt32(comando.ExecuteScalar());
                    entidad.IdReunion = nuevoId;

                    return new Response<Reunion>(true, "Reunión registrada correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Reunion>(false, $"Error al insertar: {ex.Message}", null, null);
            }
        }

        public Response<Reunion> Actualizar(Reunion entidad)
        {
            try
            {
                if (entidad == null || entidad.IdReunion <= 0)
                    return new Response<Reunion>(false, "Debe proporcionar una reunión válida", null, null);

                string sentencia = @"UPDATE [Reunion] 
                                 SET [fecha] = @fecha,
                                     [enlaceMeet] = @enlaceMeet,
                                     [estadoConfirmacion] = @estadoConfirmacion,
                                     [idCandidato] = @idCandidato,
                                     [idReclutador] = @idReclutador
                                 WHERE [idReunion] = @id;";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@fecha", entidad.Fecha);
                    comando.Parameters.AddWithValue("@enlaceMeet", entidad.EnlaceMeet ?? "");
                    comando.Parameters.AddWithValue("@estadoConfirmacion", entidad.EstadoConfirmacion ?? "Pendiente");
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idReclutador", entidad.IdReclutador);
                    comando.Parameters.AddWithValue("@id", entidad.IdReunion);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<Reunion>(true, "Reunión actualizada correctamente", entidad, null);

                    return new Response<Reunion>(false, "No se encontró la reunión especificada", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Reunion>(false, $"Error al actualizar: {ex.Message}", null, null);
            }
        }

        public Response<Reunion> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Reunion>(false, "El ID proporcionado no es válido", null, null);

                string sentencia = "DELETE FROM [Reunion] WHERE [idReunion] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<Reunion>(true, "Reunión eliminada correctamente", null, null);

                    return new Response<Reunion>(false, "No se encontró la reunión especificada", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Reunion>(false, $"Error al eliminar: {ex.Message}", null, null);
            }
        }

        public Response<Reunion> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Reunion>(false, "El ID no es válido", null, null);

                string sentencia = @"SELECT r.[idReunion], r.[fecha], r.[enlaceMeet], r.[estadoConfirmacion], 
                                 r.[idCandidato], r.[idReclutador],
                                 uc.[nombre] AS NombreCandidato, uc.[correo] AS CorreoCandidato,
                                 ur.[nombre] AS NombreReclutador, ur.[correo] AS CorreoReclutador
                                 FROM [Reunion] r
                                 INNER JOIN [Candidato] c ON r.[idCandidato] = c.[idCandidato]
                                 INNER JOIN [Usuario] uc ON c.[idCandidato] = uc.[idUsuario]
                                 INNER JOIN [Reclutador] rec ON r.[idReclutador] = rec.[idReclutador]
                                 INNER JOIN [Usuario] ur ON rec.[idReclutador] = ur.[idUsuario]
                                 WHERE r.[idReunion] = @id;";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Reunion reunion = new Reunion
                            {
                                IdReunion = reader.GetInt32(0),
                                Fecha = reader.GetDateTime(1),
                                EnlaceMeet = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                EstadoConfirmacion = reader.GetString(3),
                                IdCandidato = reader.GetInt32(4),
                                IdReclutador = reader.GetInt32(5),
                                NombreCandidato = reader.GetString(6),
                                CorreoCandidato = reader.GetString(7),
                                NombreReclutador = reader.GetString(8),
                                CorreoReclutador = reader.GetString(9)
                            };

                            return new Response<Reunion>(true, "Reunión encontrada", reunion, null);
                        }
                        return new Response<Reunion>(false, "No se encontró la reunión", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<Reunion>(false, $"Error al consultar: {ex.Message}", null, null);
            }
        }

        public Response<Reunion> ObtenerTodos()
        {
            try
            {
                IList<Reunion> lista = new List<Reunion>();

                string sentencia = @"SELECT r.[idReunion], r.[fecha], r.[enlaceMeet], r.[estadoConfirmacion], 
                                 r.[idCandidato], r.[idReclutador],
                                 uc.[nombre] AS NombreCandidato, uc.[correo] AS CorreoCandidato,
                                 ur.[nombre] AS NombreReclutador, ur.[correo] AS CorreoReclutador
                                 FROM [Reunion] r
                                 INNER JOIN [Candidato] c ON r.[idCandidato] = c.[idCandidato]
                                 INNER JOIN [Usuario] uc ON c.[idCandidato] = uc.[idUsuario]
                                 INNER JOIN [Reclutador] rec ON r.[idReclutador] = rec.[idReclutador]
                                 INNER JOIN [Usuario] ur ON rec.[idReclutador] = ur.[idUsuario]
                                 ORDER BY r.[fecha] DESC;";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Reunion
                            {
                                IdReunion = reader.GetInt32(0),
                                Fecha = reader.GetDateTime(1),
                                EnlaceMeet = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                EstadoConfirmacion = reader.GetString(3),
                                IdCandidato = reader.GetInt32(4),
                                IdReclutador = reader.GetInt32(5),
                                NombreCandidato = reader.GetString(6),
                                CorreoCandidato = reader.GetString(7),
                                NombreReclutador = reader.GetString(8),
                                CorreoReclutador = reader.GetString(9)
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Reunion>(true, $"Se encontraron {lista.Count} reuniones", null, lista);

                return new Response<Reunion>(true, "No hay reuniones registradas", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Reunion>(false, $"Error al obtener todas: {ex.Message}", null, null);
            }
        }
    }
}

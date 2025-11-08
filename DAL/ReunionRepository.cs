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

                if (entidad.IdPostulacion <= 0)
                    return new Response<Reunion>(false, "Debe especificar un IdPostulacion válido", null, null);

                string sentencia = @"
                    INSERT INTO [dbo.Reunion] ([fecha], [enlaceMeet], [estadoConfirmacion], [idPostulacion]) 
                    VALUES (@fecha, @enlaceMeet, @estadoConfirmacion, @idPostulacion);
                    SELECT SCOPE_IDENTITY();";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@fecha", entidad.Fecha);
                    comando.Parameters.AddWithValue("@enlaceMeet", entidad.EnlaceMeet ?? "");
                    comando.Parameters.AddWithValue("@estadoConfirmacion", entidad.EstadoConfirmacion ?? "Pendiente");
                    comando.Parameters.AddWithValue("@idPostulacion", entidad.IdPostulacion);

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

                string sentencia = @"
                    UPDATE [dbo.Reunion] 
                    SET [fecha] = @fecha,
                        [enlaceMeet] = @enlaceMeet,
                        [estadoConfirmacion] = @estadoConfirmacion,
                        [idPostulacion] = @idPostulacion
                    WHERE [idReunion] = @id;";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@fecha", entidad.Fecha);
                    comando.Parameters.AddWithValue("@enlaceMeet", entidad.EnlaceMeet ?? "");
                    comando.Parameters.AddWithValue("@estadoConfirmacion", entidad.EstadoConfirmacion ?? "Pendiente");
                    comando.Parameters.AddWithValue("@idPostulacion", entidad.IdPostulacion);
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

                string sentencia = "DELETE FROM [dbo.Reunion] WHERE [idReunion] = @id";

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

                string sentencia = @"
                    SELECT R.idReunion, R.fecha, R.enlaceMeet, R.estadoConfirmacion, R.idPostulacion,
                           C.nombre AS NombreCandidato, C.correo AS CorreoCandidato,
                           Re.nombre AS NombreReclutador, Re.correo AS CorreoReclutador
                    FROM Reunion R
                    INNER JOIN Postulacion P ON R.idPostulacion = P.idPostulacion
                    INNER JOIN Candidato C ON P.idCandidato = C.idCandidato
                    INNER JOIN Reclutador Re ON P.idReclutador = Re.idReclutador
                    WHERE R.idReunion = @id;";

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
                                EnlaceMeet = reader.GetString(2),
                                EstadoConfirmacion = reader.GetString(3),
                                IdPostulacion = reader.GetInt32(4),
                                NombreCandidato = reader.GetString(5),
                                CorreoCandidato = reader.GetString(6),
                                NombreReclutador = reader.GetString(7),
                                CorreoReclutador = reader.GetString(8)
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

                string sentencia = @"
                    SELECT R.idReunion, R.fecha, R.enlaceMeet, R.estadoConfirmacion, R.idPostulacion,
                           C.nombre AS NombreCandidato, C.correo AS CorreoCandidato,
                           Re.nombre AS NombreReclutador, Re.correo AS CorreoReclutador
                    FROM Reunion R
                    INNER JOIN Postulacion P ON R.idPostulacion = P.idPostulacion
                    INNER JOIN Candidato C ON P.idCandidato = C.idCandidato
                    INNER JOIN Reclutador Re ON P.idReclutador = Re.idReclutador
                    ORDER BY R.fecha DESC;";

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
                                EnlaceMeet = reader.GetString(2),
                                EstadoConfirmacion = reader.GetString(3),
                                IdPostulacion = reader.GetInt32(4),
                                NombreCandidato = reader.GetString(5),
                                CorreoCandidato = reader.GetString(6),
                                NombreReclutador = reader.GetString(7),
                                CorreoReclutador = reader.GetString(8)
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

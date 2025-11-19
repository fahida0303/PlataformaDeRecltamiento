using System;
using ENTITY;
using System.Collections.Generic;
using System.Data.SqlClient;

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

                string sentencia = @"
                    INSERT INTO [Postulacion] 
                        ([fecha_postulacion], [estado], [idCandidato], [idConvocatoria], [score], [justificacion]) 
                    VALUES 
                        (@fechaPostulacion, @estado, @idCandidato, @idConvocatoria, @score, @justificacion);
                    SELECT SCOPE_IDENTITY();";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@fechaPostulacion", entidad.FechaPostulacion);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Pendiente");
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idConvocatoria", entidad.IdConvocatoria);
                    comando.Parameters.AddWithValue("@score", entidad.Score ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@justificacion", entidad.Justificacion ?? (object)DBNull.Value);

                    conexion.Open();
                    int nuevoId = Convert.ToInt32(comando.ExecuteScalar());
                    entidad.IdPostulacion = nuevoId;

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

                string sentencia = @"
                    UPDATE [Postulacion] 
                    SET [fecha_postulacion] = @fechaPostulacion, 
                        [estado] = @estado, 
                        [idCandidato] = @idCandidato, 
                        [idConvocatoria] = @idConvocatoria,
                        [score] = @score,
                        [justificacion] = @justificacion
                    WHERE [idPostulacion] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@fechaPostulacion", entidad.FechaPostulacion);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Pendiente");
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idConvocatoria", entidad.IdConvocatoria);
                    comando.Parameters.AddWithValue("@score", entidad.Score ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@justificacion", entidad.Justificacion ?? (object)DBNull.Value);
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

                string sentencia = @"
                    SELECT [idPostulacion], [fecha_postulacion], [estado], 
                           [idCandidato], [idConvocatoria], [score], [justificacion]
                    FROM [Postulacion] 
                    WHERE [idPostulacion] = @id";

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
                                IdConvocatoria = reader.GetInt32(4),
                                Score = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                Justificacion = reader.IsDBNull(6) ? null : reader.GetString(6)
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

                string sentencia = @"
                    SELECT [idPostulacion], [fecha_postulacion], [estado], 
                           [idCandidato], [idConvocatoria], [score], [justificacion]
                    FROM [Postulacion] 
                    ORDER BY [fecha_postulacion] DESC";

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
                                IdConvocatoria = reader.GetInt32(4),
                                Score = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                Justificacion = reader.IsDBNull(6) ? null : reader.GetString(6)
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

        // ✅ NUEVO: Obtener candidatos de una convocatoria CON sus CVs en texto
        public Response<Postulacion> ObtenerCandidatosPorConvocatoria(int idConvocatoria)
        {
            try
            {
                IList<Postulacion> lista = new List<Postulacion>();

                string sentencia = @"
                    SELECT 
                        p.idPostulacion,
                        p.idCandidato,
                        p.idConvocatoria,
                        p.score,
                        p.justificacion,
                        u.nombre,
                        u.correo,
                        c.hojaDeVida
                    FROM Postulacion p
                    INNER JOIN Candidato c ON p.idCandidato = c.idCandidato
                    INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                    WHERE p.idConvocatoria = @id
                    ORDER BY p.fecha_postulacion DESC";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idConvocatoria);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Postulacion
                            {
                                IdPostulacion = reader.GetInt32(0),
                                IdCandidato = reader.GetInt32(1),
                                IdConvocatoria = reader.GetInt32(2),
                                Score = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                Justificacion = reader.IsDBNull(4) ? null : reader.GetString(4),
                                NombreCandidato = reader.GetString(5),
                                Correo = reader.GetString(6),
                                HojaDeVida = reader.IsDBNull(7) ? null : (byte[])reader["hojaDeVida"]
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Postulacion>(true, "Candidatos encontrados", null, lista);
                return new Response<Postulacion>(true, "No hay candidatos en esta convocatoria", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error: {ex.Message}", null, null);
            }
        }

        // ✅ NUEVO: Actualizar solo el score y justificación (usado por n8n)
        public Response<Postulacion> ActualizarScore(int idPostulacion, int score, string justificacion)
        {
            try
            {
                if (idPostulacion <= 0 || score < 0 || score > 100)
                {
                    return new Response<Postulacion>(false, "Datos inválidos. Score debe estar entre 0-100", null, null);
                }

                string sentencia = @"
                    UPDATE Postulacion 
                    SET score = @score, 
                        justificacion = @justificacion,
                        estado = 'Evaluado'
                    WHERE idPostulacion = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@score", score);
                    comando.Parameters.AddWithValue("@justificacion", justificacion ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@id", idPostulacion);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<Postulacion>(true, "Score actualizado correctamente", null, null);

                    return new Response<Postulacion>(false, "Postulación no encontrada", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error: {ex.Message}", null, null);
            }
        }

        // ✅ NUEVO: Obtener top 3 candidatos por score
        public Response<Postulacion> ObtenerTop3(int idConvocatoria)
        {
            try
            {
                IList<Postulacion> lista = new List<Postulacion>();

                string sentencia = @"
                    SELECT TOP 3
                        p.idPostulacion,
                        p.score,
                        p.justificacion,
                        u.nombre,
                        u.correo
                    FROM Postulacion p
                    INNER JOIN Candidato c ON p.idCandidato = c.idCandidato
                    INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                    WHERE p.idConvocatoria = @id 
                      AND p.score IS NOT NULL
                    ORDER BY p.score DESC";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idConvocatoria);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Postulacion
                            {
                                IdPostulacion = reader.GetInt32(0),
                                Score = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                Justificacion = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                NombreCandidato = reader.GetString(3),
                                Correo = reader.GetString(4)
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Postulacion>(true, "Top candidatos obtenidos", null, lista);
                return new Response<Postulacion>(false, "No hay candidatos evaluados", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}
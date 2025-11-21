using ENTITY;
using System;
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

                const string sentencia = @"
                    INSERT INTO [Postulacion]
                        ([fechaPostulacion], [estado], [idCandidato], [idConvocatoria], [score], [justificacion])
                    VALUES
                        (@fechaPostulacion, @estado, @idCandidato, @idConvocatoria, @score, @justificacion);
                    SELECT SCOPE_IDENTITY();";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@fechaPostulacion", entidad.FechaPostulacion);
                    comando.Parameters.AddWithValue("@estado", (object)entidad.Estado ?? "Pendiente");
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idConvocatoria", entidad.IdConvocatoria);
                    comando.Parameters.AddWithValue("@score", (object)entidad.Score ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@justificacion", (object)entidad.Justificacion ?? DBNull.Value);

                    conexion.Open();
                    var result = comando.ExecuteScalar();
                    entidad.IdPostulacion = Convert.ToInt32(result);

                    return new Response<Postulacion>(true, "Postulación creada correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al insertar postulación: {ex.Message}", null, null);
            }
        }

        public Response<Postulacion> Actualizar(Postulacion entidad)
        {
            try
            {
                if (entidad == null || entidad.IdPostulacion <= 0)
                    return new Response<Postulacion>(false, "Datos inválidos", null, null);

                const string sentencia = @"
                    UPDATE [Postulacion]
                    SET [fechaPostulacion] = @fechaPostulacion,
                        [estado] = @estado,
                        [idCandidato] = @idCandidato,
                        [idConvocatoria] = @idConvocatoria,
                        [score] = @score,
                        [justificacion] = @justificacion
                    WHERE [idPostulacion] = @id;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@fechaPostulacion", entidad.FechaPostulacion);
                    comando.Parameters.AddWithValue("@estado", (object)entidad.Estado ?? "Pendiente");
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idConvocatoria", entidad.IdConvocatoria);
                    comando.Parameters.AddWithValue("@score", (object)entidad.Score ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@justificacion", (object)entidad.Justificacion ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@id", entidad.IdPostulacion);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<Postulacion>(true, "Postulación actualizada correctamente", entidad, null);

                    return new Response<Postulacion>(false, "No se encontró la postulación a actualizar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al actualizar postulación: {ex.Message}", null, null);
            }
        }

        public Response<Postulacion> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Postulacion>(false, "Id inválido", null, null);

                const string sentencia = @"DELETE FROM [Postulacion] WHERE [idPostulacion] = @id;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    int filas = comando.ExecuteNonQuery();
                    if (filas > 0)
                        return new Response<Postulacion>(true, "Postulación eliminada correctamente", null, null);

                    return new Response<Postulacion>(false, "No se encontró la postulación a eliminar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al eliminar postulación: {ex.Message}", null, null);
            }
        }

        public Response<Postulacion> ObtenerPorId(int id)
        {
            try
            {
                const string sentencia = @"
                    SELECT [idPostulacion], [idCandidato], [idConvocatoria],
                           [fechaPostulacion], [estado], [score], [justificacion]
                    FROM [Postulacion]
                    WHERE [idPostulacion] = @id;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var p = new Postulacion
                            {
                                IdPostulacion = reader.GetInt32(reader.GetOrdinal("idPostulacion")),
                                IdCandidato = reader.GetInt32(reader.GetOrdinal("idCandidato")),
                                IdConvocatoria = reader.GetInt32(reader.GetOrdinal("idConvocatoria")),
                                FechaPostulacion = reader.GetDateTime(reader.GetOrdinal("fechaPostulacion")),
                                Estado = reader["estado"] as string,
                                Score = reader["score"] == DBNull.Value
                                    ? (decimal?)null
                                    : Convert.ToDecimal(reader["score"]),
                                Justificacion = reader["justificacion"] as string
                            };

                            return new Response<Postulacion>(true, "Postulación encontrada", p, null);
                        }
                    }
                }

                return new Response<Postulacion>(false, "No se encontró la postulación", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al obtener postulación: {ex.Message}", null, null);
            }
        }

        public Response<Postulacion> ObtenerTodos()
        {
            try
            {
                var lista = new List<Postulacion>();

                const string sentencia = @"
                    SELECT [idPostulacion], [idCandidato], [idConvocatoria],
                           [fechaPostulacion], [estado], [score], [justificacion]
                    FROM [Postulacion];";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var p = new Postulacion
                            {
                                IdPostulacion = reader.GetInt32(reader.GetOrdinal("idPostulacion")),
                                IdCandidato = reader.GetInt32(reader.GetOrdinal("idCandidato")),
                                IdConvocatoria = reader.GetInt32(reader.GetOrdinal("idConvocatoria")),
                                FechaPostulacion = reader.GetDateTime(reader.GetOrdinal("fechaPostulacion")),
                                Estado = reader["estado"] as string,
                                Score = reader["score"] == DBNull.Value
                                    ? (decimal?)null
                                    : Convert.ToDecimal(reader["score"]),
                                Justificacion = reader["justificacion"] as string
                            };
                            lista.Add(p);
                        }
                    }
                }

                return new Response<Postulacion>(true, $"Se encontraron {lista.Count} postulaciones", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al obtener postulaciones: {ex.Message}", null, null);
            }
        }

        // ✅ Usado por flujo n8n para leer candidatos de una convocatoria (con CV)
        public Response<Postulacion> ObtenerCandidatosPorConvocatoria(int idConvocatoria)
        {
            try
            {
                var lista = new List<Postulacion>();

                const string sentencia = @"
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
                    ORDER BY p.fechaPostulacion DESC;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idConvocatoria);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var p = new Postulacion
                            {
                                IdPostulacion = reader.GetInt32(reader.GetOrdinal("idPostulacion")),
                                IdCandidato = reader.GetInt32(reader.GetOrdinal("idCandidato")),
                                IdConvocatoria = reader.GetInt32(reader.GetOrdinal("idConvocatoria")),
                                Score = reader["score"] == DBNull.Value
                                    ? (decimal?)null
                                    : Convert.ToDecimal(reader["score"]),
                                Justificacion = reader["justificacion"] as string,
                                NombreCandidato = reader["nombre"] as string,
                                CorreoCandidato = reader["correo"] as string,
                                HojaDeVida = reader["hojaDeVida"] == DBNull.Value
                                    ? null
                                    : (byte[])reader["hojaDeVida"]
                            };

                            lista.Add(p);
                        }
                    }
                }

                return new Response<Postulacion>(true, $"Se encontraron {lista.Count} candidatos", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al obtener candidatos por convocatoria: {ex.Message}", null, null);
            }
        }

        // ✅ Usado por flujo n8n para actualizar score y justificación
        public Response<Postulacion> ActualizarScore(int idPostulacion, int score, string justificacion)
        {
            try
            {
                if (idPostulacion <= 0 || score < 0 || score > 100)
                {
                    return new Response<Postulacion>(false, "Datos inválidos. Score debe estar entre 0-100", null, null);
                }

                const string sentencia = @"
                    UPDATE Postulacion
                    SET score = @score,
                        justificacion = @justificacion
                    WHERE idPostulacion = @id;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@score", score);
                    comando.Parameters.AddWithValue("@justificacion", (object)justificacion ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@id", idPostulacion);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<Postulacion>(true, "Score actualizado correctamente", null, null);

                    return new Response<Postulacion>(false, "No se encontró la postulación para actualizar score", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al actualizar score: {ex.Message}", null, null);
            }
        }

        // ✅ Usado para tu panel de Reclutador → Top 3
        public Response<Postulacion> ObtenerTop3(int idConvocatoria)
        {
            try
            {
                var lista = new List<Postulacion>();

                const string sentencia = @"
                    SELECT TOP 3
                        p.idPostulacion,
                        p.idCandidato,
                        p.idConvocatoria,
                        p.score,
                        p.justificacion,
                        u.nombre,
                        u.correo
                    FROM Postulacion p
                    INNER JOIN Candidato c ON p.idCandidato = c.idCandidato
                    INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                    WHERE p.idConvocatoria = @id
                      AND p.score IS NOT NULL
                    ORDER BY p.score DESC;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", idConvocatoria);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var p = new Postulacion
                            {
                                IdPostulacion = reader.GetInt32(reader.GetOrdinal("idPostulacion")),
                                IdCandidato = reader.GetInt32(reader.GetOrdinal("idCandidato")),
                                IdConvocatoria = reader.GetInt32(reader.GetOrdinal("idConvocatoria")),
                                Score = reader["score"] == DBNull.Value
                                    ? (decimal?)null
                                    : Convert.ToDecimal(reader["score"]),
                                Justificacion = reader["justificacion"] as string,
                                NombreCandidato = reader["nombre"] as string,
                                CorreoCandidato = reader["correo"] as string
                            };

                            lista.Add(p);
                        }
                    }
                }

                return new Response<Postulacion>(true, $"Top 3 cargado ({lista.Count} registros)", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Postulacion>(false, $"Error al obtener Top 3: {ex.Message}", null, null);
            }
        }
    }
}

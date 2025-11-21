using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL
{
    public class HistorialEstadoRepository : BaseRepository, IRepository<HistorialEstado>
    {
        public Response<HistorialEstado> Insertar(HistorialEstado entidad)
        {
            try
            {
                if (entidad == null || entidad.IdPostulacion <= 0)
                    return new Response<HistorialEstado>(false, "IdPostulacion inválido", null, null);

                const string sentencia = @"
                    INSERT INTO [HistorialEstado]
                        ([idPostulacion], [estadoAnterior], [estadoNuevo],
                         [fechaCambio], [comentario], [usuarioCambio])
                    VALUES
                        (@idPostulacion, @estadoAnterior, @estadoNuevo,
                         @fechaCambio, @comentario, @usuarioCambio);
                    SELECT SCOPE_IDENTITY();";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idPostulacion", entidad.IdPostulacion);
                    comando.Parameters.AddWithValue("@estadoAnterior", (object)entidad.EstadoAnterior ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@estadoNuevo", (object)entidad.EstadoNuevo ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@fechaCambio", entidad.FechaCambio);
                    comando.Parameters.AddWithValue("@comentario", (object)entidad.Comentario ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@usuarioCambio", entidad.UsuarioCambio.HasValue
                        ? (object)entidad.UsuarioCambio.Value
                        : DBNull.Value);

                    conexion.Open();
                    var result = comando.ExecuteScalar();
                    entidad.IdHistorial = Convert.ToInt32(result);

                    return new Response<HistorialEstado>(true, "Historial registrado correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error al insertar historial: {ex.Message}", null, null);
            }
        }

        public Response<HistorialEstado> Actualizar(HistorialEstado entidad)
        {
            try
            {
                if (entidad == null || entidad.IdHistorial <= 0)
                    return new Response<HistorialEstado>(false, "Datos inválidos", null, null);

                const string sentencia = @"
                    UPDATE [HistorialEstado]
                    SET [idPostulacion] = @idPostulacion,
                        [estadoAnterior] = @estadoAnterior,
                        [estadoNuevo] = @estadoNuevo,
                        [fechaCambio] = @fechaCambio,
                        [comentario] = @comentario,
                        [usuarioCambio] = @usuarioCambio
                    WHERE [idHistorial] = @idHistorial;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idPostulacion", entidad.IdPostulacion);
                    comando.Parameters.AddWithValue("@estadoAnterior", (object)entidad.EstadoAnterior ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@estadoNuevo", (object)entidad.EstadoNuevo ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@fechaCambio", entidad.FechaCambio);
                    comando.Parameters.AddWithValue("@comentario", (object)entidad.Comentario ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@usuarioCambio", entidad.UsuarioCambio.HasValue
                        ? (object)entidad.UsuarioCambio.Value
                        : DBNull.Value);
                    comando.Parameters.AddWithValue("@idHistorial", entidad.IdHistorial);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<HistorialEstado>(true, "Historial actualizado correctamente", entidad, null);

                    return new Response<HistorialEstado>(false, "No se encontró el historial a actualizar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error al actualizar historial: {ex.Message}", null, null);
            }
        }

        public Response<HistorialEstado> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<HistorialEstado>(false, "Id inválido", null, null);

                const string sentencia = @"DELETE FROM [HistorialEstado] WHERE [idHistorial] = @idHistorial;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idHistorial", id);
                    conexion.Open();

                    int filas = comando.ExecuteNonQuery();
                    if (filas > 0)
                        return new Response<HistorialEstado>(true, "Historial eliminado correctamente", null, null);

                    return new Response<HistorialEstado>(false, "No se encontró el historial a eliminar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error al eliminar historial: {ex.Message}", null, null);
            }
        }

        public Response<HistorialEstado> ObtenerPorId(int id)
        {
            try
            {
                const string sentencia = @"
                    SELECT [idHistorial], [idPostulacion], [estadoAnterior],
                           [estadoNuevo], [fechaCambio], [comentario], [usuarioCambio]
                    FROM [HistorialEstado]
                    WHERE [idHistorial] = @idHistorial;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idHistorial", id);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var h = new HistorialEstado
                            {
                                IdHistorial = reader.GetInt32(reader.GetOrdinal("idHistorial")),
                                IdPostulacion = reader.GetInt32(reader.GetOrdinal("idPostulacion")),
                                EstadoAnterior = reader["estadoAnterior"] as string,
                                EstadoNuevo = reader["estadoNuevo"] as string,
                                FechaCambio = reader.GetDateTime(reader.GetOrdinal("fechaCambio")),
                                Comentario = reader["comentario"] as string,
                                UsuarioCambio = reader["usuarioCambio"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(reader["usuarioCambio"])
                            };

                            return new Response<HistorialEstado>(true, "Historial encontrado", h, null);
                        }
                    }
                }

                return new Response<HistorialEstado>(false, "No se encontró el historial", null, null);
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error al obtener historial: {ex.Message}", null, null);
            }
        }

        public Response<HistorialEstado> ObtenerTodos()
        {
            try
            {
                var lista = new List<HistorialEstado>();

                const string sentencia = @"
                    SELECT [idHistorial], [idPostulacion], [estadoAnterior],
                           [estadoNuevo], [fechaCambio], [comentario], [usuarioCambio]
                    FROM [HistorialEstado];";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var h = new HistorialEstado
                            {
                                IdHistorial = reader.GetInt32(reader.GetOrdinal("idHistorial")),
                                IdPostulacion = reader.GetInt32(reader.GetOrdinal("idPostulacion")),
                                EstadoAnterior = reader["estadoAnterior"] as string,
                                EstadoNuevo = reader["estadoNuevo"] as string,
                                FechaCambio = reader.GetDateTime(reader.GetOrdinal("fechaCambio")),
                                Comentario = reader["comentario"] as string,
                                UsuarioCambio = reader["usuarioCambio"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(reader["usuarioCambio"])
                            };

                            lista.Add(h);
                        }
                    }
                }

                return new Response<HistorialEstado>(true, $"Se encontraron {lista.Count} historiales", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error al obtener historiales: {ex.Message}", null, null);
            }
        }
    }
}

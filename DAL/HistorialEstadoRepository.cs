using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class HistorialEstadoRepository : BaseRepository, IRepository<HistorialEstado>
    {
        public Response<HistorialEstado> Insertar(HistorialEstado entidad)
        {
            try
            {
                if (entidad == null || entidad.IdPostulacion <= 0)
                {
                    return new Response<HistorialEstado>(false, "Los datos son inválidos", null, null);
                }

                string sentencia = @"INSERT INTO [HistorialEstado] ([idPostulacion], [estadoAnterior], [estadoNuevo], 
                                 [fechaCambio], [comentario], [usuarioCambio]) 
                                 VALUES (@idPostulacion, @estadoAnterior, @estadoNuevo, @fechaCambio, @comentario, @usuarioCambio)";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idPostulacion", entidad.IdPostulacion);
                    comando.Parameters.AddWithValue("@estadoAnterior", entidad.EstadoAnterior);
                    comando.Parameters.AddWithValue("@estadoNuevo", entidad.EstadoNuevo);
                    comando.Parameters.AddWithValue("@fechaCambio", entidad.FechaCambio);
                    comando.Parameters.AddWithValue("@comentario", entidad.Comentario ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@usuarioCambio", entidad.UsuarioCambio.HasValue ? (object)entidad.UsuarioCambio.Value : DBNull.Value);

                    conexion.Open();
                    comando.ExecuteNonQuery();

                    return new Response<HistorialEstado>(true, "Historial insertado correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<HistorialEstado> Actualizar(HistorialEstado entidad)
        {
            try
            {
                if (entidad == null || entidad.IdHistorial <= 0)
                {
                    return new Response<HistorialEstado>(false, "Datos inválidos", null, null);
                }

                string sentencia = @"UPDATE [HistorialEstado] SET [idPostulacion] = @idPostulacion, 
                                 [estadoAnterior] = @estadoAnterior, [estadoNuevo] = @estadoNuevo, 
                                 [fechaCambio] = @fechaCambio, [comentario] = @comentario, [usuarioCambio] = @usuarioCambio 
                                 WHERE [idHistorial] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idPostulacion", entidad.IdPostulacion);
                    comando.Parameters.AddWithValue("@estadoAnterior", entidad.EstadoAnterior);
                    comando.Parameters.AddWithValue("@estadoNuevo", entidad.EstadoNuevo);
                    comando.Parameters.AddWithValue("@fechaCambio", entidad.FechaCambio);
                    comando.Parameters.AddWithValue("@comentario", entidad.Comentario ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@usuarioCambio", entidad.UsuarioCambio.HasValue ? (object)entidad.UsuarioCambio.Value : DBNull.Value);
                    comando.Parameters.AddWithValue("@id", entidad.IdHistorial);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<HistorialEstado>(true, "Actualizado correctamente", entidad, null);
                    return new Response<HistorialEstado>(false, "No se encontró el registro", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<HistorialEstado> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<HistorialEstado>(false, "El ID debe ser mayor a cero", null, null);

                string sentencia = "DELETE FROM [HistorialEstado] WHERE [idHistorial] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<HistorialEstado>(true, "Eliminado correctamente", null, null);
                    return new Response<HistorialEstado>(false, "No se encontró el registro", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<HistorialEstado> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<HistorialEstado>(false, "El ID debe ser mayor a cero", null, null);

                string sentencia = @"SELECT [idHistorial], [idPostulacion], [estadoAnterior], [estadoNuevo], 
                                 [fechaCambio], [comentario], [usuarioCambio] 
                                 FROM [HistorialEstado] WHERE [idHistorial] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            HistorialEstado historial = new HistorialEstado
                            {
                                IdHistorial = reader.GetInt32(0),
                                IdPostulacion = reader.GetInt32(1),
                                EstadoAnterior = reader.GetString(2),
                                EstadoNuevo = reader.GetString(3),
                                FechaCambio = reader.GetDateTime(4),
                                Comentario = reader.IsDBNull(5) ? null : reader.GetString(5),
                                UsuarioCambio = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6)
                            };
                            return new Response<HistorialEstado>(true, "Encontrado", historial, null);
                        }
                        return new Response<HistorialEstado>(false, "No encontrado", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<HistorialEstado> ObtenerTodos()
        {
            try
            {
                IList<HistorialEstado> lista = new List<HistorialEstado>();
                string sentencia = @"SELECT [idHistorial], [idPostulacion], [estadoAnterior], [estadoNuevo], 
                                 [fechaCambio], [comentario], [usuarioCambio] 
                                 FROM [HistorialEstado] ORDER BY [fechaCambio] DESC";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new HistorialEstado
                            {
                                IdHistorial = reader.GetInt32(0),
                                IdPostulacion = reader.GetInt32(1),
                                EstadoAnterior = reader.GetString(2),
                                EstadoNuevo = reader.GetString(3),
                                FechaCambio = reader.GetDateTime(4),
                                Comentario = reader.IsDBNull(5) ? null : reader.GetString(5),
                                UsuarioCambio = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6)
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<HistorialEstado>(true, $"Se encontraron {lista.Count}", null, lista);
                return new Response<HistorialEstado>(true, "Sin registros", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<HistorialEstado>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

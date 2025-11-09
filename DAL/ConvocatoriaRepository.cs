using System;
using ENTITY;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ConvocatoriaRepository : BaseRepository, IRepository<Convocatoria>
    {
        public Response<Convocatoria> Insertar(Convocatoria entidad)
        {
            try
            {
                if (entidad == null || string.IsNullOrWhiteSpace(entidad.Titulo))
                {
                    return new Response<Convocatoria>(false, "El título de la convocatoria es requerido", null, null);
                }

                string sentencia = @"INSERT INTO [Convocatoria] ([titulo], [descripcion], [fechaPublicacion], [fechaLimite], [estado], [idEmpresa], [idReclutador]) 
                                 VALUES (@titulo, @descripcion, @fechaPublicacion, @fechaLimite, @estado, @idEmpresa, @idReclutador)";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@titulo", entidad.Titulo);
                    comando.Parameters.AddWithValue("@descripcion", entidad.Descripcion);
                    comando.Parameters.AddWithValue("@fechaPublicacion", entidad.FechaPublicacion);
                    comando.Parameters.AddWithValue("@fechaLimite", entidad.FechaLimite);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Abierta");
                    comando.Parameters.AddWithValue("@idEmpresa", entidad.IdEmpresa);
                    comando.Parameters.AddWithValue("@idReclutador", entidad.IdReclutador.HasValue ? (object)entidad.IdReclutador.Value : DBNull.Value);

                    conexion.Open();
                    comando.ExecuteNonQuery();

                    return new Response<Convocatoria>(true, "Convocatoria insertada correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Convocatoria> Actualizar(Convocatoria entidad)
        {
            try
            {
                if (entidad == null || entidad.IdConvocatoria <= 0)
                {
                    return new Response<Convocatoria>(false, "Datos inválidos para actualizar", null, null);
                }

                string sentencia = @"UPDATE [Convocatoria] SET [titulo] = @titulo, [descripcion] = @descripcion, 
                                 [fechaPublicacion] = @fechaPublicacion, [fechaLimite] = @fechaLimite, 
                                 [estado] = @estado, [idEmpresa] = @idEmpresa, [idReclutador] = @idReclutador
                                 WHERE [idConvocatoria] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@titulo", entidad.Titulo);
                    comando.Parameters.AddWithValue("@descripcion", entidad.Descripcion);
                    comando.Parameters.AddWithValue("@fechaPublicacion", entidad.FechaPublicacion);
                    comando.Parameters.AddWithValue("@fechaLimite", entidad.FechaLimite);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Abierta");
                    comando.Parameters.AddWithValue("@idEmpresa", entidad.IdEmpresa);
                    comando.Parameters.AddWithValue("@idReclutador", entidad.IdReclutador.HasValue ? (object)entidad.IdReclutador.Value : DBNull.Value);
                    comando.Parameters.AddWithValue("@id", entidad.IdConvocatoria);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Convocatoria>(true, "Convocatoria actualizada correctamente", entidad, null);
                    return new Response<Convocatoria>(false, "No se encontró la convocatoria para actualizar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Convocatoria> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Convocatoria>(false, "El ID debe ser mayor a cero", null, null);

                string sentencia = "DELETE FROM [Convocatoria] WHERE [idConvocatoria] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Convocatoria>(true, "Convocatoria eliminada correctamente", null, null);
                    return new Response<Convocatoria>(false, "No se encontró la convocatoria con el ID especificado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Convocatoria> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Convocatoria>(false, "El ID debe ser mayor a cero", null, null);

                string sentencia = @"SELECT [idConvocatoria], [titulo], [descripcion], [fechaPublicacion], [fechaLimite], [estado], [idEmpresa], [idReclutador] 
                                 FROM [Convocatoria] WHERE [idConvocatoria] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Convocatoria convocatoria = new Convocatoria
                            {
                                IdConvocatoria = reader.GetInt32(0),
                                Titulo = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                FechaPublicacion = reader.GetDateTime(3),
                                FechaLimite = reader.GetDateTime(4),
                                Estado = reader.GetString(5),
                                IdEmpresa = reader.GetInt32(6),
                                IdReclutador = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7)
                            };
                            return new Response<Convocatoria>(true, "Convocatoria encontrada", convocatoria, null);
                        }
                        return new Response<Convocatoria>(false, "No se encontró la convocatoria con el ID especificado", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Convocatoria> ObtenerTodos()
        {
            try
            {
                IList<Convocatoria> lista = new List<Convocatoria>();
                string sentencia = @"SELECT [idConvocatoria], [titulo], [descripcion], [fechaPublicacion], [fechaLimite], [estado], [idEmpresa], [idReclutador] 
                                 FROM [Convocatoria] ORDER BY [fechaPublicacion] DESC";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Convocatoria
                            {
                                IdConvocatoria = reader.GetInt32(0),
                                Titulo = reader.GetString(1),
                                Descripcion = reader.GetString(2),
                                FechaPublicacion = reader.GetDateTime(3),
                                FechaLimite = reader.GetDateTime(4),
                                Estado = reader.GetString(5),
                                IdEmpresa = reader.GetInt32(6),
                                IdReclutador = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7)
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Convocatoria>(true, $"Se encontraron {lista.Count}", null, lista);
                return new Response<Convocatoria>(true, "No hay convocatorias registradas", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

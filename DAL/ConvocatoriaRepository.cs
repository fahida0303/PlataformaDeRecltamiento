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

                string sentencia = @"INSERT INTO [dbo.Convocatoria] ([titulo], [descripcion], [fechaPublicacion], [idReclutador], [estado], [fechaLimite]) 
                                     VALUES (@titulo, @descripcion, @fechaPublicacion, @idReclutador, @estado, @fechaLimite); SELECT SCOPE_IDENTITY();";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@titulo", entidad.Titulo);
                    comando.Parameters.AddWithValue("@descripcion", entidad.Descripcion ?? "");
                    comando.Parameters.AddWithValue("@fechaPublicacion", entidad.FechaPublicacion);
                    comando.Parameters.AddWithValue("@idReclutador", entidad.IdReclutador);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Activo");
                    comando.Parameters.AddWithValue("@fechaLimite", entidad.FechaLimite);

                    conexion.Open();
                    int nuevoId = Convert.ToInt32(comando.ExecuteScalar());
                    entidad.IdConvocatoria = nuevoId;

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

                string sentencia = @"UPDATE [dbo.Convocatoria] SET [titulo] = @titulo, [descripcion] = @descripcion, 
                                     [fechaPublicacion] = @fechaPublicacion, [idReclutador] = @idReclutador, [estado] = @estado, [fechaLimite] = @fechaLimite 
                                     WHERE [idConvocatoria] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@titulo", entidad.Titulo ?? "");
                    comando.Parameters.AddWithValue("@descripcion", entidad.Descripcion ?? "");
                    comando.Parameters.AddWithValue("@fechaPublicacion", entidad.FechaPublicacion);
                    comando.Parameters.AddWithValue("@idReclutador", entidad.IdReclutador);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Activo");
                    comando.Parameters.AddWithValue("@fechaLimite", entidad.FechaLimite);
                    comando.Parameters.AddWithValue("@id", entidad.IdConvocatoria);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<Convocatoria>(true, "Convocatoria actualizada correctamente", entidad, null);
                    }
                    else
                    {
                        return new Response<Convocatoria>(false, "No se encontró la convocatoria para actualizar", null, null);
                    }
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
                {
                    return new Response<Convocatoria>(false, "El ID debe ser mayor a cero", null, null);
                }

                string sentencia = "DELETE FROM [dbo.Convocatoria] WHERE [idConvocatoria] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<Convocatoria>(true, "Convocatoria eliminada correctamente", null, null);
                    }
                    else
                    {
                        return new Response<Convocatoria>(false, "No se encontró la convocatoria con el ID especificado", null, null);
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    return new Response<Convocatoria>(false, "No se puede eliminar: la convocatoria está siendo utilizada", null, null);
                }
                return new Response<Convocatoria>(false, $"Error en la base de datos: {ex.Message}", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error al eliminar la convocatoria: {ex.Message}", null, null);
            }
        }

        public Response<Convocatoria> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Convocatoria>(false, "El ID debe ser mayor a cero", null, null);
                }

                string sentencia = @"SELECT [idConvocatoria], [titulo], [descripcion], [fechaPublicacion], [idReclutador], [estado], [fechaLimite] 
                                     FROM [dbo.Convocatoria] WHERE [idConvocatoria] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);

                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Convocatoria convocatoria = MapearConvocatoria(reader);
                            return new Response<Convocatoria>(true, "Convocatoria encontrada", convocatoria, null);
                        }
                        else
                        {
                            return new Response<Convocatoria>(false, "No se encontró la convocatoria con el ID especificado", null, null);
                        }
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
                IList<Convocatoria> listaConvocatorias = new List<Convocatoria>();
                string sentencia = @"SELECT [idConvocatoria], [titulo], [descripcion], [fechaPublicacion], [idReclutador], [estado], [fechaLimite] 
                                     FROM [dbo.Convocatoria] ORDER BY [fechaPublicacion] DESC";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaConvocatorias.Add(MapearConvocatoria(reader));
                        }
                    }
                }

                if (listaConvocatorias.Count > 0)
                {
                    return new Response<Convocatoria>(true, $"Se encontraron {listaConvocatorias.Count} convocatorias", null, listaConvocatorias);
                }
                else
                {
                    return new Response<Convocatoria>(true, "No hay convocatorias registradas", null, listaConvocatorias);
                }
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error: {ex.Message}", null, null);
            }
        }

        private Convocatoria MapearConvocatoria(SqlDataReader reader)
        {
            return new Convocatoria
            {
                IdConvocatoria = reader.GetInt32(0),
                Titulo = reader.GetString(1),
                Descripcion = reader.GetString(2),
                FechaPublicacion = reader.GetDateTime(3),
                IdReclutador = reader.GetInt32(4),
                Estado = reader.GetString(5),
                FechaLimite = reader.GetDateTime(6)
            };
        }
    }
}

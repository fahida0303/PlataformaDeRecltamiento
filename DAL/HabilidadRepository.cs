using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class HabilidadRepository : BaseRepository, IRepository<Habilidad>
    {
        public Response<Habilidad> Insertar(Habilidad entidad)
        {
            try
            {
                if (entidad == null || string.IsNullOrWhiteSpace(entidad.Nombre))
                {
                    return new Response<Habilidad>(false, "El nombre de la habilidad es requerido", null, null);
                }

                string sentencia = @"INSERT INTO [Habilidad] ([Nombre], [Categoria]) 
                                     VALUES (@nombre, @categoria); SELECT SCOPE_IDENTITY();";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@categoria", entidad.Categoria ?? "");

                    conexion.Open();
                    int nuevoId = Convert.ToInt32(comando.ExecuteScalar());
                    entidad.IdHabilidad = nuevoId;

                    return new Response<Habilidad>(true, "Habilidad insertada correctamente", entidad, null);
                }
            }
            catch (SqlException ex)
            {
                return new Response<Habilidad>(false, $"Error en la base de datos: {ex.Message}", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error al insertar la habilidad: {ex.Message}", null, null);
            }
        }

        public Response<Habilidad> Actualizar(Habilidad entidad)
        {
            try
            {
                if (entidad == null || entidad.IdHabilidad <= 0)
                {
                    return new Response<Habilidad>(false, "Datos inválidos para actualizar", null, null);
                }

                string sentencia = @"UPDATE [Habilidad] SET [Nombre] = @nombre, [Categoria] = @categoria 
                                     WHERE [IdHabilidad] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre ?? "");
                    comando.Parameters.AddWithValue("@categoria", entidad.Categoria ?? "");
                    comando.Parameters.AddWithValue("@id", entidad.IdHabilidad);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<Habilidad>(true, "Habilidad actualizada correctamente", entidad, null);
                    }
                    else
                    {
                        return new Response<Habilidad>(false, "No se encontró la habilidad para actualizar", null, null);
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<Habilidad>(false, $"Error en la base de datos: {ex.Message}", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error al actualizar la habilidad: {ex.Message}", null, null);
            }
        }

        public Response<Habilidad> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Habilidad>(false, "El ID debe ser mayor a cero", null, null);
                }

                string sentencia = "DELETE FROM [Habilidad] WHERE [IdHabilidad] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<Habilidad>(true, "Habilidad eliminada correctamente", null, null);
                    }
                    else
                    {
                        return new Response<Habilidad>(false, "No se encontró la habilidad", null, null);
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    return new Response<Habilidad>(false, "No se puede eliminar: la habilidad está siendo utilizada", null, null);
                }
                return new Response<Habilidad>(false, $"Error en la base de datos: {ex.Message}", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error al eliminar la habilidad: {ex.Message}", null, null);
            }
        }

        public Response<Habilidad> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<Habilidad>(false, "El ID debe ser mayor a cero", null, null);
                }

                string sentencia = @"SELECT [IdHabilidad], [Nombre], [Categoria] 
                                     FROM [Habilidad] WHERE [IdHabilidad] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Habilidad habilidad = MapearHabilidad(reader);
                            return new Response<Habilidad>(true, "Habilidad encontrada", habilidad, null);
                        }
                        else
                        {
                            return new Response<Habilidad>(false, "No se encontró la habilidad", null, null);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<Habilidad>(false, $"Error en la base de datos: {ex.Message}", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error al obtener la habilidad: {ex.Message}", null, null);
            }
        }

        public Response<Habilidad> ObtenerTodos()
        {
            try
            {
                IList<Habilidad> listaHabilidades = new List<Habilidad>();
                string sentencia = @"SELECT [IdHabilidad], [Nombre], [Categoria] 
                                     FROM [Habilidad] ORDER BY [Nombre]";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listaHabilidades.Add(MapearHabilidad(reader));
                        }
                    }
                }

                if (listaHabilidades.Count > 0)
                {
                    return new Response<Habilidad>(true, $"Se encontraron {listaHabilidades.Count} habilidades", null, listaHabilidades);
                }
                else
                {
                    return new Response<Habilidad>(true, "No hay habilidades registradas", null, listaHabilidades);
                }
            }
            catch (SqlException ex)
            {
                return new Response<Habilidad>(false, $"Error en la base de datos: {ex.Message}", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error al obtener las habilidades: {ex.Message}", null, null);
            }
        }

        private Habilidad MapearHabilidad(SqlDataReader reader)
        {
            return new Habilidad
            {
                IdHabilidad = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Categoria = reader.GetString(2)
            };
        }
    }
}

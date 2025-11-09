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

                string sentencia = @"INSERT INTO [Habilidad] ([nombre], [categoria]) 
                                 VALUES (@nombre, @categoria)";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@categoria", entidad.Categoria ?? (object)DBNull.Value);

                    conexion.Open();
                    comando.ExecuteNonQuery();

                    return new Response<Habilidad>(true, "Habilidad insertada correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error: {ex.Message}", null, null);
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

                string sentencia = @"UPDATE [Habilidad] SET [nombre] = @nombre, [categoria] = @categoria 
                                 WHERE [idHabilidad] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@categoria", entidad.Categoria ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@id", entidad.IdHabilidad);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Habilidad>(true, "Habilidad actualizada correctamente", entidad, null);
                    return new Response<Habilidad>(false, "No se encontró la habilidad para actualizar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Habilidad> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Habilidad>(false, "El ID debe ser mayor a cero", null, null);

                string sentencia = "DELETE FROM [Habilidad] WHERE [idHabilidad] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Habilidad>(true, "Habilidad eliminada correctamente", null, null);
                    return new Response<Habilidad>(false, "No se encontró la habilidad", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Habilidad> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Habilidad>(false, "El ID debe ser mayor a cero", null, null);

                string sentencia = @"SELECT [idHabilidad], [nombre], [categoria] 
                                 FROM [Habilidad] WHERE [idHabilidad] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Habilidad habilidad = new Habilidad
                            {
                                IdHabilidad = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Categoria = reader.IsDBNull(2) ? null : reader.GetString(2)
                            };
                            return new Response<Habilidad>(true, "Habilidad encontrada", habilidad, null);
                        }
                        return new Response<Habilidad>(false, "No se encontró la habilidad", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Habilidad> ObtenerTodos()
        {
            try
            {
                IList<Habilidad> lista = new List<Habilidad>();
                string sentencia = @"SELECT [idHabilidad], [nombre], [categoria] 
                                 FROM [Habilidad] ORDER BY [nombre]";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Habilidad
                            {
                                IdHabilidad = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Categoria = reader.IsDBNull(2) ? null : reader.GetString(2)
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Habilidad>(true, $"Se encontraron {lista.Count}", null, lista);
                return new Response<Habilidad>(true, "No hay habilidades registradas", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

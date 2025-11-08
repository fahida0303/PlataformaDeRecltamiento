using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class CandidatoHabilidadRepository : BaseRepository, IRepository<CandidatoHabilidad>
    {
        public Response<CandidatoHabilidad> Insertar(CandidatoHabilidad entidad)
        {
            try
            {
                if (entidad == null || entidad.IdCandidato <= 0 || entidad.IdHabilidad <= 0)
                {
                    return new Response<CandidatoHabilidad>(false, "Los datos son inválidos", null, null);
                }

                string sentencia = @"INSERT INTO [CandidatoHabilidad] ([IdCandidato], [IdHabilidad], [NivelDominio]) 
                                     VALUES (@idCandidato, @idHabilidad, @nivelDominio); SELECT SCOPE_IDENTITY();";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idHabilidad", entidad.IdHabilidad);
                    comando.Parameters.AddWithValue("@nivelDominio", entidad.NivelDominio ?? "");

                    conexion.Open();
                    int nuevoId = Convert.ToInt32(comando.ExecuteScalar());
                    entidad.IdCandidatoHabilidad = nuevoId;

                    return new Response<CandidatoHabilidad>(true, "Habilidad del candidato insertada correctamente", entidad, null);
                }
            }
            catch (SqlException ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error en la base de datos: {ex.Message}", null, null);
            }
            catch (Exception ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error al insertar: {ex.Message}", null, null);
            }
        }

        public Response<CandidatoHabilidad> Actualizar(CandidatoHabilidad entidad)
        {
            try
            {
                if (entidad == null || entidad.IdCandidatoHabilidad <= 0)
                {
                    return new Response<CandidatoHabilidad>(false, "Datos inválidos", null, null);
                }

                string sentencia = @"UPDATE [CandidatoHabilidad] SET [IdCandidato] = @idCandidato, 
                                     [IdHabilidad] = @idHabilidad, [NivelDominio] = @nivelDominio 
                                     WHERE [IdCandidatoHabilidad] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idHabilidad", entidad.IdHabilidad);
                    comando.Parameters.AddWithValue("@nivelDominio", entidad.NivelDominio ?? "");
                    comando.Parameters.AddWithValue("@id", entidad.IdCandidatoHabilidad);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<CandidatoHabilidad>(true, "Actualizado correctamente", entidad, null);
                    }
                    else
                    {
                        return new Response<CandidatoHabilidad>(false, "No se encontró el registro", null, null);
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error en la base de datos: {ex.Message}", null, null);
            }
            catch (Exception ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error al actualizar: {ex.Message}", null, null);
            }
        }

        public Response<CandidatoHabilidad> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<CandidatoHabilidad>(false, "El ID debe ser mayor a cero", null, null);
                }

                string sentencia = "DELETE FROM [CandidatoHabilidad] WHERE [IdCandidatoHabilidad] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        return new Response<CandidatoHabilidad>(true, "Eliminado correctamente", null, null);
                    }
                    else
                    {
                        return new Response<CandidatoHabilidad>(false, "No se encontró el registro", null, null);
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error en la base de datos: {ex.Message}", null, null);
            }
            catch (Exception ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error al eliminar: {ex.Message}", null, null);
            }
        }

        public Response<CandidatoHabilidad> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new Response<CandidatoHabilidad>(false, "El ID debe ser mayor a cero", null, null);
                }

                string sentencia = @"SELECT [IdCandidatoHabilidad], [IdCandidato], [IdHabilidad], [NivelDominio] 
                                     FROM [CandidatoHabilidad] WHERE [IdCandidatoHabilidad] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CandidatoHabilidad candidatoHabilidad = MapearCandidatoHabilidad(reader);
                            return new Response<CandidatoHabilidad>(true, "Encontrado", candidatoHabilidad, null);
                        }
                        else
                        {
                            return new Response<CandidatoHabilidad>(false, "No encontrado", null, null);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error en la base de datos: {ex.Message}", null, null);
            }
            catch (Exception ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<CandidatoHabilidad> ObtenerTodos()
        {
            try
            {
                IList<CandidatoHabilidad> lista = new List<CandidatoHabilidad>();
                string sentencia = @"SELECT [IdCandidatoHabilidad], [IdCandidato], [IdHabilidad], [NivelDominio] 
                                     FROM [CandidatoHabilidad] ORDER BY [IdCandidato]";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearCandidatoHabilidad(reader));
                        }
                    }
                }

                return new Response<CandidatoHabilidad>(true, $"Se encontraron {lista.Count} registros", null, lista);
            }
            catch (SqlException ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error en la base de datos: {ex.Message}", null, null);
            }
            catch (Exception ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error: {ex.Message}", null, null);
            }
        }

        private CandidatoHabilidad MapearCandidatoHabilidad(SqlDataReader reader)
        {
            return new CandidatoHabilidad
            {
                IdCandidatoHabilidad = reader.GetInt32(0),
                IdCandidato = reader.GetInt32(1),
                IdHabilidad = reader.GetInt32(2),
                NivelDominio = reader.GetString(3)
            };
        }
    }
}

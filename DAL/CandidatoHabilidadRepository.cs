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

                string sentencia = @"INSERT INTO [CandidatoHabilidad] ([idCandidato], [idHabilidad], [nivelDominio]) 
                                 VALUES (@idCandidato, @idHabilidad, @nivelDominio)";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idHabilidad", entidad.IdHabilidad);
                    comando.Parameters.AddWithValue("@nivelDominio", entidad.NivelDominio ?? (object)DBNull.Value);

                    conexion.Open();
                    comando.ExecuteNonQuery();

                    return new Response<CandidatoHabilidad>(true, "Habilidad del candidato insertada correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error: {ex.Message}", null, null);
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

                string sentencia = @"UPDATE [CandidatoHabilidad] SET [idCandidato] = @idCandidato, 
                                 [idHabilidad] = @idHabilidad, [nivelDominio] = @nivelDominio 
                                 WHERE [idCandidatoHabilidad] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idHabilidad", entidad.IdHabilidad);
                    comando.Parameters.AddWithValue("@nivelDominio", entidad.NivelDominio ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@id", entidad.IdCandidatoHabilidad);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<CandidatoHabilidad>(true, "Actualizado correctamente", entidad, null);
                    return new Response<CandidatoHabilidad>(false, "No se encontró el registro", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<CandidatoHabilidad> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<CandidatoHabilidad>(false, "El ID debe ser mayor a cero", null, null);

                string sentencia = "DELETE FROM [CandidatoHabilidad] WHERE [idCandidatoHabilidad] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<CandidatoHabilidad>(true, "Eliminado correctamente", null, null);
                    return new Response<CandidatoHabilidad>(false, "No se encontró el registro", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<CandidatoHabilidad> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<CandidatoHabilidad>(false, "El ID debe ser mayor a cero", null, null);

                string sentencia = @"SELECT [idCandidatoHabilidad], [idCandidato], [idHabilidad], [nivelDominio] 
                                 FROM [CandidatoHabilidad] WHERE [idCandidatoHabilidad] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CandidatoHabilidad candidatoHabilidad = new CandidatoHabilidad
                            {
                                IdCandidatoHabilidad = reader.GetInt32(0),
                                IdCandidato = reader.GetInt32(1),
                                IdHabilidad = reader.GetInt32(2),
                                NivelDominio = reader.IsDBNull(3) ? null : reader.GetString(3)
                            };
                            return new Response<CandidatoHabilidad>(true, "Encontrado", candidatoHabilidad, null);
                        }
                        return new Response<CandidatoHabilidad>(false, "No encontrado", null, null);
                    }
                }
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
                string sentencia = @"SELECT [idCandidatoHabilidad], [idCandidato], [idHabilidad], [nivelDominio] 
                                 FROM [CandidatoHabilidad] ORDER BY [idCandidato]";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new CandidatoHabilidad
                            {
                                IdCandidatoHabilidad = reader.GetInt32(0),
                                IdCandidato = reader.GetInt32(1),
                                IdHabilidad = reader.GetInt32(2),
                                NivelDominio = reader.IsDBNull(3) ? null : reader.GetString(3)
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<CandidatoHabilidad>(true, $"Se encontraron {lista.Count}", null, lista);
                return new Response<CandidatoHabilidad>(true, "Sin registros", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

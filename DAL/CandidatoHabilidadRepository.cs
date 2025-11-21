using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL
{
    public class CandidatoHabilidadRepository : BaseRepository, IRepository<CandidatoHabilidad>
    {
        public Response<CandidatoHabilidad> Insertar(CandidatoHabilidad entidad)
        {
            try
            {
                if (entidad == null || entidad.IdCandidato <= 0 || entidad.IdHabilidad <= 0)
                    return new Response<CandidatoHabilidad>(false, "Datos inválidos de candidato/habilidad", null, null);

                const string sentencia = @"
                    INSERT INTO [CandidatoHabilidad] ([idCandidato], [idHabilidad], [nivelDominio])
                    VALUES (@idCandidato, @idHabilidad, @nivelDominio);
                    SELECT SCOPE_IDENTITY();";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idHabilidad", entidad.IdHabilidad);
                    comando.Parameters.AddWithValue("@nivelDominio", (object)entidad.NivelDominio ?? DBNull.Value);

                    conexion.Open();
                    var result = comando.ExecuteScalar();
                    entidad.IdCandidatoHabilidad = Convert.ToInt32(result);

                    return new Response<CandidatoHabilidad>(true, "Registro de habilidad de candidato creado", entidad, null);
                }
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
                    return new Response<CandidatoHabilidad>(false, "Datos inválidos", null, null);

                const string sentencia = @"
                    UPDATE [CandidatoHabilidad]
                    SET [idCandidato] = @idCandidato,
                        [idHabilidad] = @idHabilidad,
                        [nivelDominio] = @nivelDominio
                    WHERE [idCandidatoHabilidad] = @id;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);
                    comando.Parameters.AddWithValue("@idHabilidad", entidad.IdHabilidad);
                    comando.Parameters.AddWithValue("@nivelDominio", (object)entidad.NivelDominio ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@id", entidad.IdCandidatoHabilidad);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<CandidatoHabilidad>(true, "Registro actualizado correctamente", entidad, null);

                    return new Response<CandidatoHabilidad>(false, "No se encontró el registro a actualizar", null, null);
                }
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
                    return new Response<CandidatoHabilidad>(false, "Id inválido", null, null);

                const string sentencia = @"DELETE FROM [CandidatoHabilidad] WHERE [idCandidatoHabilidad] = @id;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    int filas = comando.ExecuteNonQuery();
                    if (filas > 0)
                        return new Response<CandidatoHabilidad>(true, "Registro eliminado correctamente", null, null);

                    return new Response<CandidatoHabilidad>(false, "No se encontró el registro a eliminar", null, null);
                }
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
                const string sentencia = @"
                    SELECT [idCandidatoHabilidad], [idCandidato], [idHabilidad], [nivelDominio]
                    FROM [CandidatoHabilidad]
                    WHERE [idCandidatoHabilidad] = @id;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var ch = new CandidatoHabilidad
                            {
                                IdCandidatoHabilidad = reader.GetInt32(reader.GetOrdinal("idCandidatoHabilidad")),
                                IdCandidato = reader.GetInt32(reader.GetOrdinal("idCandidato")),
                                IdHabilidad = reader.GetInt32(reader.GetOrdinal("idHabilidad")),
                                NivelDominio = reader["nivelDominio"] as string
                            };

                            return new Response<CandidatoHabilidad>(true, "Registro encontrado", ch, null);
                        }
                    }
                }

                return new Response<CandidatoHabilidad>(false, "No se encontró el registro", null, null);
            }
            catch (Exception ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error al obtener por id: {ex.Message}", null, null);
            }
        }

        public Response<CandidatoHabilidad> ObtenerTodos()
        {
            try
            {
                var lista = new List<CandidatoHabilidad>();

                const string sentencia = @"
                    SELECT [idCandidatoHabilidad], [idCandidato], [idHabilidad], [nivelDominio]
                    FROM [CandidatoHabilidad];";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var ch = new CandidatoHabilidad
                            {
                                IdCandidatoHabilidad = reader.GetInt32(reader.GetOrdinal("idCandidatoHabilidad")),
                                IdCandidato = reader.GetInt32(reader.GetOrdinal("idCandidato")),
                                IdHabilidad = reader.GetInt32(reader.GetOrdinal("idHabilidad")),
                                NivelDominio = reader["nivelDominio"] as string
                            };
                            lista.Add(ch);
                        }
                    }
                }

                return new Response<CandidatoHabilidad>(true, $"Se encontraron {lista.Count} registros", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<CandidatoHabilidad>(false, $"Error al obtener todos: {ex.Message}", null, null);
            }
        }
    }
}

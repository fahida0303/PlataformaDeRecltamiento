using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ENTITY;

namespace DAL
{
    public class CandidatoRepository : BaseRepository, IRepository<Candidato>
    {
        // INSERTAR
        public Response<Candidato> Insertar(Candidato entidad)
        {
            try
            {
                if (entidad == null || entidad.IdUsuario <= 0)
                {
                    return new Response<Candidato>(false, "Datos inválidos de candidato", null, null);
                }

                string sentencia = @"
                    INSERT INTO [Candidato]
                        ([idCandidato], [tipox], [nivelFormacion], [experiencia], [hojaDeVida])
                    VALUES
                        (@idCandidato, @tipox, @nivelFormacion, @experiencia, @hojaDeVida);";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdUsuario);
                    comando.Parameters.AddWithValue("@tipox", (object)entidad.Tipox ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@nivelFormacion", (object)entidad.NivelFormacion ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@experiencia", (object)entidad.Experiencia ?? DBNull.Value);

                    // hojaDeVida como VARBINARY(MAX)
                    SqlParameter pHoja = new SqlParameter("@hojaDeVida", SqlDbType.VarBinary);
                    if (entidad.HojaDeVida != null)
                        pHoja.Value = entidad.HojaDeVida;
                    else
                        pHoja.Value = DBNull.Value;
                    comando.Parameters.Add(pHoja);

                    conexion.Open();
                    comando.ExecuteNonQuery();

                    return new Response<Candidato>(true, "Candidato insertado correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }

        // ACTUALIZAR
        public Response<Candidato> Actualizar(Candidato entidad)
        {
            try
            {
                if (entidad == null || entidad.IdUsuario <= 0)
                {
                    return new Response<Candidato>(false, "Datos inválidos", null, null);
                }

                string sentencia = @"
                    UPDATE [Candidato]
                    SET [tipox] = @tipox,
                        [nivelFormacion] = @nivelFormacion,
                        [experiencia] = @experiencia,
                        [hojaDeVida] = @hojaDeVida
                    WHERE [idCandidato] = @idCandidato;";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@tipox", (object)entidad.Tipox ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@nivelFormacion", (object)entidad.NivelFormacion ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@experiencia", (object)entidad.Experiencia ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdUsuario);

                    SqlParameter pHoja = new SqlParameter("@hojaDeVida", SqlDbType.VarBinary);
                    if (entidad.HojaDeVida != null)
                        pHoja.Value = entidad.HojaDeVida;
                    else
                        pHoja.Value = DBNull.Value;
                    comando.Parameters.Add(pHoja);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<Candidato>(true, "Candidato actualizado", entidad, null);

                    return new Response<Candidato>(false, "Candidato no encontrado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }

        // ELIMINAR
        public Response<Candidato> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Candidato>(false, "ID inválido", null, null);

                string sentencia = "DELETE FROM [Candidato] WHERE [idCandidato] = @id;";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<Candidato>(true, "Candidato eliminado", null, null);

                    return new Response<Candidato>(false, "Candidato no encontrado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }

        // OBTENER POR ID
        public Response<Candidato> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Candidato>(false, "ID inválido", null, null);

                string sentencia = @"
                    SELECT 
                        c.[idCandidato],      -- 0
                        u.[nombre],           -- 1
                        u.[correo],           -- 2
                        u.[contraseña],       -- 3
                        u.[estado],           -- 4
                        c.[tipox],            -- 5
                        c.[nivelFormacion],   -- 6
                        c.[experiencia],      -- 7
                        c.[hojaDeVida]        -- 8
                    FROM [Usuario] u
                    INNER JOIN [Candidato] c ON u.[idUsuario] = c.[idCandidato]
                    WHERE c.[idCandidato] = @id;";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Candidato cand = new Candidato
                            {
                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Correo = reader.GetString(2),
                                Contrasena = reader.GetString(3),
                                Estado = reader.GetString(4),
                                Tipox = reader.IsDBNull(5) ? null : reader.GetString(5),
                                NivelFormacion = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Experiencia = reader.IsDBNull(7) ? null : reader.GetString(7),
                                HojaDeVida = reader.IsDBNull(8) ? null : (byte[])reader["hojaDeVida"]
                            };

                            return new Response<Candidato>(true, "Candidato encontrado", cand, null);
                        }

                        return new Response<Candidato>(false, "Candidato no encontrado", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }

        // OBTENER TODOS
        public Response<Candidato> ObtenerTodos()
        {
            try
            {
                IList<Candidato> lista = new List<Candidato>();

                string sentencia = @"
                    SELECT 
                        c.[idCandidato],      -- 0
                        u.[nombre],           -- 1
                        u.[correo],           -- 2
                        u.[contraseña],       -- 3
                        u.[estado],           -- 4
                        c.[tipox],            -- 5
                        c.[nivelFormacion],   -- 6
                        c.[experiencia],      -- 7
                        c.[hojaDeVida]        -- 8
                    FROM [Usuario] u
                    INNER JOIN [Candidato] c ON u.[idUsuario] = c.[idCandidato]
                    ORDER BY u.[nombre];";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Candidato cand = new Candidato
                            {
                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Correo = reader.GetString(2),
                                Contrasena = reader.GetString(3),
                                Estado = reader.GetString(4),
                                Tipox = reader.IsDBNull(5) ? null : reader.GetString(5),
                                NivelFormacion = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Experiencia = reader.IsDBNull(7) ? null : reader.GetString(7),
                                HojaDeVida = reader.IsDBNull(8) ? null : (byte[])reader["hojaDeVida"]
                            };

                            lista.Add(cand);
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Candidato>(true, $"Se encontraron {lista.Count} candidatos", null, lista);

                return new Response<Candidato>(true, "No hay candidatos registrados", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

using ENTITY;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class CandidatoRepository : BaseRepository, IRepository<Candidato>
    {
        // ================= INSERTAR (CORREGIDO) =================
        public Response<Candidato> Insertar(Candidato entidad)
        {
            try
            {
                if (entidad == null || entidad.IdUsuario <= 0)
                    return new Response<Candidato>(false, "La entidad Candidato requiere un IdUsuario válido", null, null);

                // 🚨 CAMBIO CRÍTICO:
                // 1. Agregamos 'idCandidato' a la lista de columnas.
                // 2. Quitamos 'SELECT SCOPE_IDENTITY()' porque el ID ya lo tenemos (es el IdUsuario).
                const string sentencia = @"
                    INSERT INTO Candidato (idCandidato, tipox, nivelFormacion, experiencia, hojaDeVida)
                    VALUES (@idCandidato, @tipox, @nivelFormacion, @experiencia, @hojaDeVida);";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    // 🚨 ASIGNAMOS EL ID EXPLÍCITAMENTE
                    // Usamos entidad.IdUsuario porque en la relación 1 a 1, son el mismo número.
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdUsuario);

                    comando.Parameters.AddWithValue("@tipox", (object)entidad.Tipox ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@nivelFormacion", (object)entidad.NivelFormacion ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@experiencia", (object)entidad.Experiencia ?? DBNull.Value);

                    // Manejo de binario para PDF
                    var pHoja = new SqlParameter("@hojaDeVida", SqlDbType.VarBinary)
                    {
                        Value = (object)entidad.HojaDeVida ?? DBNull.Value
                    };
                    comando.Parameters.Add(pHoja);

                    conexion.Open();
                    comando.ExecuteNonQuery(); // Ya no necesitamos leer el ID de vuelta

                    // Sincronizamos el IdCandidato en el objeto por si acaso
                    entidad.IdCandidato = entidad.IdUsuario;

                    return new Response<Candidato>(true, "Candidato insertado correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error al insertar candidato: {ex.Message}", null, null);
            }
        }

        // ================= ACTUALIZAR =================
        public Response<Candidato> Actualizar(Candidato entidad)
        {
            try
            {
                if (entidad == null || entidad.IdCandidato <= 0)
                    return new Response<Candidato>(false, "Datos inválidos para actualizar candidato", null, null);

                const string sentencia = @"
                    UPDATE Candidato
                    SET tipox          = @tipox,
                        nivelFormacion = @nivelFormacion,
                        experiencia    = @experiencia,
                        hojaDeVida     = @hojaDeVida
                    WHERE idCandidato  = @idCandidato;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@tipox", (object)entidad.Tipox ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@nivelFormacion", (object)entidad.NivelFormacion ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@experiencia", (object)entidad.Experiencia ?? DBNull.Value);

                    var pHoja = new SqlParameter("@hojaDeVida", SqlDbType.VarBinary)
                    {
                        Value = (object)entidad.HojaDeVida ?? DBNull.Value
                    };
                    comando.Parameters.Add(pHoja);

                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdCandidato);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<Candidato>(true, "Candidato actualizado correctamente", entidad, null);

                    return new Response<Candidato>(false, "No se encontró el candidato a actualizar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error al actualizar candidato: {ex.Message}", null, null);
            }
        }

        // ================= ELIMINAR =================
        public Response<Candidato> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Candidato>(false, "Id de candidato inválido", null, null);

                const string sentencia = @"DELETE FROM Candidato WHERE idCandidato = @idCandidato;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idCandidato", id);
                    conexion.Open();

                    int filas = comando.ExecuteNonQuery();
                    if (filas > 0)
                        return new Response<Candidato>(true, "Candidato eliminado correctamente", null, null);

                    return new Response<Candidato>(false, "No se encontró el candidato a eliminar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error al eliminar candidato: {ex.Message}", null, null);
            }
        }

        // ================= OBTENER POR ID =================
        public Response<Candidato> ObtenerPorId(int id)
        {
            try
            {
                const string sentencia = @"
                    SELECT 
                        idCandidato,
                        tipox,
                        nivelFormacion,
                        experiencia,
                        hojaDeVida
                    FROM Candidato
                    WHERE idCandidato = @idCandidato;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idCandidato", id);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var cand = new Candidato
                            {
                                IdCandidato    = reader.GetInt32(reader.GetOrdinal("idCandidato")),
                                Tipox          = reader["tipox"] as string,
                                NivelFormacion = reader["nivelFormacion"] as string,
                                Experiencia    = reader["experiencia"] as string,
                                HojaDeVida     = reader["hojaDeVida"] == DBNull.Value
                                                    ? null
                                                    : (byte[])reader["hojaDeVida"],
                                IdUsuario      = reader.GetInt32(reader.GetOrdinal("idCandidato")) // Mapeo implícito
                            };

                            return new Response<Candidato>(true, "Candidato encontrado", cand, null);
                        }
                    }
                }

                return new Response<Candidato>(false, "No se encontró el candidato", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error al obtener candidato: {ex.Message}", null, null);
            }
        }

        // ================= OBTENER TODOS =================
        public Response<Candidato> ObtenerTodos()
        {
            try
            {
                var lista = new List<Candidato>();

                const string sentencia = @"
                    SELECT 
                        idCandidato,
                        tipox,
                        nivelFormacion,
                        experiencia,
                        hojaDeVida
                    FROM Candidato;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cand = new Candidato
                            {
                                IdCandidato    = reader.GetInt32(reader.GetOrdinal("idCandidato")),
                                Tipox          = reader["tipox"] as string,
                                NivelFormacion = reader["nivelFormacion"] as string,
                                Experiencia    = reader["experiencia"] as string,
                                HojaDeVida     = reader["hojaDeVida"] == DBNull.Value
                                                    ? null
                                                    : (byte[])reader["hojaDeVida"],
                                IdUsuario      = reader.GetInt32(reader.GetOrdinal("idCandidato"))
                            };

                            lista.Add(cand);
                        }
                    }
                }

                return new Response<Candidato>(true, $"Se encontraron {lista.Count} candidatos", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error al obtener candidatos: {ex.Message}", null, null);
            }
        }
    }
}
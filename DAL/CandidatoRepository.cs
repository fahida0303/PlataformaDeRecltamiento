using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class CandidatoRepository : BaseRepository, IRepository<Candidato>
    {
        public Response<Candidato> Insertar(Candidato entidad)
        {
            try
            {
                if (entidad == null || string.IsNullOrWhiteSpace(entidad.Nombre))
                {
                    return new Response<Candidato>(false, "El nombre es requerido", null, null);
                }

                string sentencia = @"INSERT INTO [Candidato] ([idCandidato], [tipox], [nivelFormacion], [experiencia], [hojaDeVida]) 
                                 VALUES (@idCandidato, @tipox, @nivelFormacion, @experiencia, @hojaDeVida)";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdUsuario);
                    comando.Parameters.AddWithValue("@tipox", entidad.Tipox ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@nivelFormacion", entidad.NivelFormacion ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@experiencia", entidad.Experiencia ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@hojaDeVida", entidad.HojaDeVida ?? (object)DBNull.Value);

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

        public Response<Candidato> Actualizar(Candidato entidad)
        {
            try
            {
                if (entidad == null || entidad.IdUsuario <= 0)
                {
                    return new Response<Candidato>(false, "Datos inválidos", null, null);
                }

                string sentencia = @"UPDATE [Candidato] SET [tipox] = @tipox, [nivelFormacion] = @nivelFormacion,
                                 [experiencia] = @experiencia, [hojaDeVida] = @hojaDeVida
                                 WHERE [idCandidato] = @idCandidato";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@tipox", entidad.Tipox ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@nivelFormacion", entidad.NivelFormacion ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@experiencia", entidad.Experiencia ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@hojaDeVida", entidad.HojaDeVida ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@idCandidato", entidad.IdUsuario);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Candidato>(true, "Actualizado", entidad, null);
                    return new Response<Candidato>(false, "No encontrado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Candidato> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Candidato>(false, "ID inválido", null, null);

                string sentencia = "DELETE FROM [Candidato] WHERE [idCandidato] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Candidato>(true, "Eliminado", null, null);
                    return new Response<Candidato>(false, "No encontrado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Candidato> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Candidato>(false, "ID inválido", null, null);

                string sentencia = @"SELECT c.[idCandidato], u.[nombre], u.[correo], u.[contraseña], u.[estado], 
                                 c.[tipox], c.[nivelFormacion], c.[experiencia], c.[hojaDeVida]
                                 FROM [Usuario] u
                                 INNER JOIN [Candidato] c ON u.[idUsuario] = c.[idCandidato]
                                 WHERE c.[idCandidato] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Candidato candidato = new Candidato
                            {
                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Correo = reader.GetString(2),
                                Contrasena = reader.GetString(3),
                                Estado = reader.GetString(4),
                                Tipox = reader.IsDBNull(5) ? null : reader.GetString(5),
                                NivelFormacion = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Experiencia = reader.IsDBNull(7) ? null : reader.GetString(7),
                                HojaDeVida = reader.IsDBNull(8) ? null : reader.GetString(8)
                            };
                            return new Response<Candidato>(true, "Encontrado", candidato, null);
                        }
                        return new Response<Candidato>(false, "No encontrado", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Candidato> ObtenerTodos()
        {
            try
            {
                IList<Candidato> lista = new List<Candidato>();
                string sentencia = @"SELECT c.[idCandidato], u.[nombre], u.[correo], u.[contraseña], u.[estado],
                                 c.[tipox], c.[nivelFormacion], c.[experiencia], c.[hojaDeVida]
                                 FROM [Usuario] u
                                 INNER JOIN [Candidato] c ON u.[idUsuario] = c.[idCandidato]
                                 ORDER BY u.[nombre]";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Candidato
                            {
                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Correo = reader.GetString(2),
                                Contrasena = reader.GetString(3),
                                Estado = reader.GetString(4),
                                Tipox = reader.IsDBNull(5) ? null : reader.GetString(5),
                                NivelFormacion = reader.IsDBNull(6) ? null : reader.GetString(6),
                                Experiencia = reader.IsDBNull(7) ? null : reader.GetString(7),
                                HojaDeVida = reader.IsDBNull(8) ? null : reader.GetString(8)
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Candidato>(true, $"Se encontraron {lista.Count}", null, lista);
                return new Response<Candidato>(true, "Sin registros", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Candidato>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

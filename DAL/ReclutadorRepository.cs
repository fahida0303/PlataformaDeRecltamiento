using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ReclutadorRepository : BaseRepository, IRepository<Reclutador>
    {
        public Response<Reclutador> Insertar(Reclutador entidad)
        {
            try
            {
                if (entidad == null || string.IsNullOrWhiteSpace(entidad.Nombre))
                {
                    return new Response<Reclutador>(false, "El nombre es requerido", null, null);
                }

                string sentencia = @"INSERT INTO [Reclutador] ([idReclutador], [cargo], [empresaAsociada]) 
                                 VALUES (@idReclutador, @cargo, @empresaAsociada)";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idReclutador", entidad.IdUsuario);
                    comando.Parameters.AddWithValue("@cargo", entidad.Cargo ?? "");
                    comando.Parameters.AddWithValue("@empresaAsociada", entidad.IdEmpresa);

                    conexion.Open();
                    comando.ExecuteNonQuery();

                    return new Response<Reclutador>(true, "Reclutador insertado correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Reclutador> Actualizar(Reclutador entidad)
        {
            try
            {
                if (entidad == null || entidad.IdUsuario <= 0)
                {
                    return new Response<Reclutador>(false, "Datos inválidos", null, null);
                }

                string sentencia = @"UPDATE [Reclutador] SET [cargo] = @cargo, [empresaAsociada] = @empresaAsociada
                                 WHERE [idReclutador] = @idReclutador";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@cargo", entidad.Cargo ?? "");
                    comando.Parameters.AddWithValue("@empresaAsociada", entidad.IdEmpresa);
                    comando.Parameters.AddWithValue("@idReclutador", entidad.IdUsuario);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Reclutador>(true, "Actualizado", entidad, null);
                    return new Response<Reclutador>(false, "No encontrado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Reclutador> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Reclutador>(false, "ID inválido", null, null);

                // El trigger TR_Reclutador_DesactivarUsuario se encargará de poner estado = 'Inactivo' en Usuario
                string sentencia = "DELETE FROM [Reclutador] WHERE [idReclutador] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Reclutador>(true, "Eliminado", null, null);
                    return new Response<Reclutador>(false, "No encontrado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Reclutador> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Reclutador>(false, "ID inválido", null, null);

                string sentencia = @"SELECT r.[idReclutador], u.[nombre], u.[correo], u.[contraseña], u.[estado], 
                                 r.[cargo], r.[empresaAsociada]
                                 FROM [Usuario] u
                                 INNER JOIN [Reclutador] r ON u.[idUsuario] = r.[idReclutador]
                                 WHERE r.[idReclutador] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Reclutador reclutador = new Reclutador
                            {
                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Correo = reader.GetString(2),
                                Contrasena = reader.GetString(3),
                                Estado = reader.GetString(4),
                                Cargo = reader.GetString(5),
                                IdEmpresa = reader.GetInt32(6)
                            };
                            return new Response<Reclutador>(true, "Encontrado", reclutador, null);
                        }
                        return new Response<Reclutador>(false, "No encontrado", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Reclutador> ObtenerTodos()
        {
            try
            {
                IList<Reclutador> lista = new List<Reclutador>();
                string sentencia = @"SELECT r.[idReclutador], u.[nombre], u.[correo], u.[contraseña], u.[estado],
                                 r.[cargo], r.[empresaAsociada]
                                 FROM [Usuario] u
                                 INNER JOIN [Reclutador] r ON u.[idUsuario] = r.[idReclutador]
                                 ORDER BY u.[nombre]";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Reclutador
                            {
                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Correo = reader.GetString(2),
                                Contrasena = reader.GetString(3),
                                Estado = reader.GetString(4),
                                Cargo = reader.GetString(5),
                                IdEmpresa = reader.GetInt32(6)
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Reclutador>(true, $"Se encontraron {lista.Count}", null, lista);
                return new Response<Reclutador>(true, "Sin registros", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

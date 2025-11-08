using System;
using ENTITY;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class EmpresaRepository : BaseRepository, IRepository<Empresa>
    {
        public Response<Empresa> Insertar(Empresa entidad)
        {
            try
            {
                if (entidad == null || string.IsNullOrWhiteSpace(entidad.Nombre))
                {
                    return new Response<Empresa>(false, "El nombre es requerido", null, null);
                }

                string sentencia = @"INSERT INTO [dbo.Empresa] ([nombre], [sector], [direccion], [correoContacto], [telefono], [sitioWeb], [fechaRegistro]) 
                                     VALUES (@nombre, @sector, @direccion, @correoContacto, @telefono, @sitioWeb, @fechaRegistro); SELECT SCOPE_IDENTITY();";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@sector", entidad.Sector ?? "");
                    comando.Parameters.AddWithValue("@direccion", entidad.Direccion ?? "");
                    comando.Parameters.AddWithValue("@correoContacto", entidad.CorreoContacto ?? "");
                    comando.Parameters.AddWithValue("@telefono", entidad.Telefono ?? "");
                    comando.Parameters.AddWithValue("@sitioWeb", entidad.SitioWeb ?? "");
                    comando.Parameters.AddWithValue("@fechaRegistro", DateTime.Now);

                    conexion.Open();
                    int nuevoId = Convert.ToInt32(comando.ExecuteScalar());
                    entidad.IdEmpresa = nuevoId;

                    return new Response<Empresa>(true, "Empresa insertada correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> Actualizar(Empresa entidad)
        {
            try
            {
                if (entidad == null || entidad.IdEmpresa <= 0)
                {
                    return new Response<Empresa>(false, "Datos inválidos", null, null);
                }

                string sentencia = @"UPDATE [dbo.Empresa] SET [nombre] = @nombre, [sector] = @sector, [direccion] = @direccion, 
                                     [correoContacto] = @correoContacto, [telefono] = @telefono, [sitioWeb] = @sitioWeb, [fechaActualizacion] = @fechaActualizacion
                                     WHERE [idEmpresa] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre ?? "");
                    comando.Parameters.AddWithValue("@sector", entidad.Sector ?? "");
                    comando.Parameters.AddWithValue("@direccion", entidad.Direccion ?? "");
                    comando.Parameters.AddWithValue("@correoContacto", entidad.CorreoContacto ?? "");
                    comando.Parameters.AddWithValue("@telefono", entidad.Telefono ?? "");
                    comando.Parameters.AddWithValue("@sitioWeb", entidad.SitioWeb ?? "");
                    comando.Parameters.AddWithValue("@fechaActualizacion", DateTime.Now);
                    comando.Parameters.AddWithValue("@id", entidad.IdEmpresa);

                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Empresa>(true, "Actualizado correctamente", entidad, null);
                    return new Response<Empresa>(false, "No encontrado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Empresa>(false, "ID inválido", null, null);

                string sentencia = "DELETE FROM [dbo.Empresa] WHERE [idEmpresa] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                        return new Response<Empresa>(true, "Eliminado correctamente", null, null);
                    return new Response<Empresa>(false, "No encontrado", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> ObtenerPorId(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Empresa>(false, "ID inválido", null, null);

                string sentencia = @"SELECT [idEmpresa], [nombre], [sector], [direccion], [correoContacto], [telefono], [sitioWeb], [fechaRegistro], [fechaActualizacion]
                                     FROM [dbo.Empresa]
                                     WHERE [idEmpresa] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Empresa empresa = new Empresa
                            {
                                IdEmpresa = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Sector = reader.GetString(2),
                                Direccion = reader.GetString(3),
                                CorreoContacto = reader.GetString(4),
                                Telefono = reader.GetString(5),
                                SitioWeb = reader.GetString(6),
                                FechaRegistro = reader.GetDateTime(7),
                                FechaActualizacion = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8)
                            };
                            return new Response<Empresa>(true, "Encontrado", empresa, null);
                        }
                        return new Response<Empresa>(false, "No encontrado", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> ObtenerTodos()
        {
            try
            {
                IList<Empresa> lista = new List<Empresa>();
                string sentencia = @"SELECT [idEmpresa], [nombre], [sector], [direccion], [correoContacto], [telefono], [sitioWeb], [fechaRegistro], [fechaActualizacion]
                                     FROM [dbo.Empresa]
                                     ORDER BY [nombre]";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Empresa
                            {
                                IdEmpresa = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Sector = reader.GetString(2),
                                Direccion = reader.GetString(3),
                                CorreoContacto = reader.GetString(4),
                                Telefono = reader.GetString(5),
                                SitioWeb = reader.GetString(6),
                                FechaRegistro = reader.GetDateTime(7),
                                FechaActualizacion = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8)
                            });
                        }
                    }
                }

                if (lista.Count > 0)
                    return new Response<Empresa>(true, $"Se encontraron {lista.Count}", null, lista);
                return new Response<Empresa>(true, "Sin registros", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error: {ex.Message}", null, null);
            }
        }
    }
}

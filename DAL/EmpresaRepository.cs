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

                string sentencia = @"INSERT INTO [Empresa] ([nombre], [sector], [direccion], [correo_contacto]) 
                                 VALUES (@nombre, @sector, @direccion, @correoContacto)";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@sector", entidad.Sector);
                    comando.Parameters.AddWithValue("@direccion", entidad.Direccion ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@correoContacto", entidad.CorreoContacto);

                    conexion.Open();
                    comando.ExecuteNonQuery();

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

                string sentencia = @"UPDATE [Empresa] SET [nombre] = @nombre, [sector] = @sector, 
                                 [direccion] = @direccion, [correo_contacto] = @correoContacto
                                 WHERE [idEmpresa] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@sector", entidad.Sector);
                    comando.Parameters.AddWithValue("@direccion", entidad.Direccion ?? (object)DBNull.Value);
                    comando.Parameters.AddWithValue("@correoContacto", entidad.CorreoContacto);
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

                string sentencia = "DELETE FROM [Empresa] WHERE [idEmpresa] = @id";

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

                string sentencia = @"SELECT [idEmpresa], [nombre], [sector], [direccion], [correo_contacto]
                                 FROM [Empresa]
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
                                Direccion = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CorreoContacto = reader.GetString(4)
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
                string sentencia = @"SELECT [idEmpresa], [nombre], [sector], [direccion], [correo_contacto]
                                 FROM [Empresa]
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
                                Direccion = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CorreoContacto = reader.GetString(4)
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

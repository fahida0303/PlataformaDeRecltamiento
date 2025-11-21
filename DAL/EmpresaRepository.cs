using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL
{
    public class EmpresaRepository : BaseRepository, IRepository<Empresa>
    {
        public Response<Empresa> Insertar(Empresa entidad)
        {
            try
            {
                if (entidad == null || string.IsNullOrWhiteSpace(entidad.Nombre))
                    return new Response<Empresa>(false, "El nombre de la empresa es obligatorio", null, null);

                const string sentencia = @"
                    INSERT INTO [Empresa] ([nombre], [sector], [direccion], [correoContacto])
                    VALUES (@nombre, @sector, @direccion, @correoContacto);
                    SELECT SCOPE_IDENTITY();";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@sector", (object)entidad.Sector ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@direccion", (object)entidad.Direccion ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@correoContacto", (object)entidad.CorreoContacto ?? DBNull.Value);

                    conexion.Open();
                    var result = comando.ExecuteScalar();
                    entidad.IdEmpresa = Convert.ToInt32(result);

                    return new Response<Empresa>(true, "Empresa creada correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error al insertar empresa: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> Actualizar(Empresa entidad)
        {
            try
            {
                if (entidad == null || entidad.IdEmpresa <= 0)
                    return new Response<Empresa>(false, "Datos inválidos", null, null);

                const string sentencia = @"
                    UPDATE [Empresa]
                    SET [nombre] = @nombre,
                        [sector] = @sector,
                        [direccion] = @direccion,
                        [correoContacto] = @correoContacto
                    WHERE [idEmpresa] = @idEmpresa;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@sector", (object)entidad.Sector ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@direccion", (object)entidad.Direccion ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@correoContacto", (object)entidad.CorreoContacto ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@idEmpresa", entidad.IdEmpresa);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<Empresa>(true, "Empresa actualizada correctamente", entidad, null);

                    return new Response<Empresa>(false, "No se encontró la empresa a actualizar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error al actualizar empresa: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Empresa>(false, "Id inválido", null, null);

                const string sentencia = @"DELETE FROM [Empresa] WHERE [idEmpresa] = @idEmpresa;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idEmpresa", id);
                    conexion.Open();

                    int filas = comando.ExecuteNonQuery();
                    if (filas > 0)
                        return new Response<Empresa>(true, "Empresa eliminada correctamente", null, null);

                    return new Response<Empresa>(false, "No se encontró la empresa a eliminar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error al eliminar empresa: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> ObtenerPorId(int id)
        {
            try
            {
                const string sentencia = @"
                    SELECT [idEmpresa], [nombre], [sector], [direccion], [correoContacto]
                    FROM [Empresa]
                    WHERE [idEmpresa] = @idEmpresa;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idEmpresa", id);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var empresa = new Empresa
                            {
                                IdEmpresa = reader.GetInt32(reader.GetOrdinal("idEmpresa")),
                                Nombre = reader["nombre"] as string,
                                Sector = reader["sector"] as string,
                                Direccion = reader["direccion"] as string,
                                CorreoContacto = reader["correoContacto"] as string
                            };

                            return new Response<Empresa>(true, "Empresa encontrada", empresa, null);
                        }
                    }
                }

                return new Response<Empresa>(false, "No se encontró la empresa", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error al obtener empresa: {ex.Message}", null, null);
            }
        }

        public Response<Empresa> ObtenerTodos()
        {
            try
            {
                var lista = new List<Empresa>();

                const string sentencia = @"
                    SELECT [idEmpresa], [nombre], [sector], [direccion], [correoContacto]
                    FROM [Empresa];";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var empresa = new Empresa
                            {
                                IdEmpresa = reader.GetInt32(reader.GetOrdinal("idEmpresa")),
                                Nombre = reader["nombre"] as string,
                                Sector = reader["sector"] as string,
                                Direccion = reader["direccion"] as string,
                                CorreoContacto = reader["correoContacto"] as string
                            };
                            lista.Add(empresa);
                        }
                    }
                }

                return new Response<Empresa>(true, $"Se encontraron {lista.Count} empresas", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Empresa>(false, $"Error al obtener empresas: {ex.Message}", null, null);
            }
        }
    }
}

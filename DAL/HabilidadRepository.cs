using ENTITY;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL
{
    public class HabilidadRepository : BaseRepository, IRepository<Habilidad>
    {
        public Response<Habilidad> Insertar(Habilidad entidad)
        {
            try
            {
                if (entidad == null || string.IsNullOrWhiteSpace(entidad.Nombre))
                    return new Response<Habilidad>(false, "El nombre de la habilidad es obligatorio", null, null);

                const string sentencia = @"
                    INSERT INTO [Habilidad] ([nombre], [categoria])
                    VALUES (@nombre, @categoria);
                    SELECT SCOPE_IDENTITY();";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@categoria", (object)entidad.Categoria ?? DBNull.Value);

                    conexion.Open();
                    var result = comando.ExecuteScalar();
                    entidad.IdHabilidad = Convert.ToInt32(result);

                    return new Response<Habilidad>(true, "Habilidad creada correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error al insertar habilidad: {ex.Message}", null, null);
            }
        }

        public Response<Habilidad> Actualizar(Habilidad entidad)
        {
            try
            {
                if (entidad == null || entidad.IdHabilidad <= 0)
                    return new Response<Habilidad>(false, "Datos inválidos", null, null);

                const string sentencia = @"
                    UPDATE [Habilidad]
                    SET [nombre] = @nombre,
                        [categoria] = @categoria
                    WHERE [idHabilidad] = @idHabilidad;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", entidad.Nombre);
                    comando.Parameters.AddWithValue("@categoria", (object)entidad.Categoria ?? DBNull.Value);
                    comando.Parameters.AddWithValue("@idHabilidad", entidad.IdHabilidad);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                        return new Response<Habilidad>(true, "Habilidad actualizada correctamente", entidad, null);

                    return new Response<Habilidad>(false, "No se encontró la habilidad a actualizar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error al actualizar habilidad: {ex.Message}", null, null);
            }
        }

        public Response<Habilidad> Eliminar(int id)
        {
            try
            {
                if (id <= 0)
                    return new Response<Habilidad>(false, "Id inválido", null, null);

                const string sentencia = @"DELETE FROM [Habilidad] WHERE [idHabilidad] = @idHabilidad;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idHabilidad", id);
                    conexion.Open();

                    int filas = comando.ExecuteNonQuery();
                    if (filas > 0)
                        return new Response<Habilidad>(true, "Habilidad eliminada correctamente", null, null);

                    return new Response<Habilidad>(false, "No se encontró la habilidad a eliminar", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error al eliminar habilidad: {ex.Message}", null, null);
            }
        }

        public Response<Habilidad> ObtenerPorId(int id)
        {
            try
            {
                const string sentencia = @"
                    SELECT [idHabilidad], [nombre], [categoria]
                    FROM [Habilidad]
                    WHERE [idHabilidad] = @idHabilidad;";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@idHabilidad", id);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var hab = new Habilidad
                            {
                                IdHabilidad = reader.GetInt32(reader.GetOrdinal("idHabilidad")),
                                Nombre = reader["nombre"] as string,
                                Categoria = reader["categoria"] as string
                            };

                            return new Response<Habilidad>(true, "Habilidad encontrada", hab, null);
                        }
                    }
                }

                return new Response<Habilidad>(false, "No se encontró la habilidad", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error al obtener habilidad: {ex.Message}", null, null);
            }
        }

        public Response<Habilidad> ObtenerTodos()
        {
            try
            {
                var lista = new List<Habilidad>();

                const string sentencia = @"
                    SELECT [idHabilidad], [nombre], [categoria]
                    FROM [Habilidad];";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var hab = new Habilidad
                            {
                                IdHabilidad = reader.GetInt32(reader.GetOrdinal("idHabilidad")),
                                Nombre = reader["nombre"] as string,
                                Categoria = reader["categoria"] as string
                            };
                            lista.Add(hab);
                        }
                    }
                }

                return new Response<Habilidad>(true, $"Se encontraron {lista.Count} habilidades", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Habilidad>(false, $"Error al obtener habilidades: {ex.Message}", null, null);
            }
        }
    }
}

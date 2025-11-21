using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ENTITY;

namespace DAL
{
    public class ReclutadorRepository : BaseRepository, IRepository<Reclutador>
    {
        // ==========================
        // INSERTAR
        // ==========================
        public Response<Reclutador> Insertar(Reclutador entidad)
        {
            try
            {
                // Validaciones básicas
                if (entidad == null || entidad.IdUsuario <= 0)
                {
                    return new Response<Reclutador>(false, "Datos inválidos: Se requiere un IdUsuario previo.", null, null);
                }

                // 🚨 CORRECCIÓN CLAVE: 
                // 1. Usamos el nombre real de la columna FK: idEmpresa
                // 2. Insertamos el ID explícitamente (idReclutador = idUsuario)
                string sentencia = @"
                    INSERT INTO [Reclutador] ([idReclutador], [cargo], [idEmpresa]) 
                    VALUES (@id, @cargo, @idEmpresa);";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", entidad.IdUsuario);
                    comando.Parameters.AddWithValue("@cargo", entidad.Cargo ?? "Reclutador");
                    comando.Parameters.AddWithValue("@idEmpresa", entidad.IdEmpresa);

                    conexion.Open();
                    comando.ExecuteNonQuery();

                    // Sincronizar IDs en el objeto
                    entidad.IdReclutador = entidad.IdUsuario;

                    return new Response<Reclutador>(true, "Reclutador creado correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error al insertar Reclutador: {ex.Message}", null, null);
            }
        }

        // ==========================
        // ACTUALIZAR
        // ==========================
        public Response<Reclutador> Actualizar(Reclutador entidad)
        {
            try
            {
                if (entidad == null || entidad.IdReclutador <= 0)
                {
                    return new Response<Reclutador>(false, "Datos inválidos para actualizar", null, null);
                }

                // 🚨 CORRECCIÓN: idEmpresa en lugar de empresaAsociada
                string sentencia = @"
                    UPDATE [Reclutador] 
                    SET [cargo] = @cargo, 
                        [idEmpresa] = @idEmpresa
                    WHERE [idReclutador] = @id;";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@cargo", entidad.Cargo ?? "");
                    comando.Parameters.AddWithValue("@idEmpresa", entidad.IdEmpresa);
                    comando.Parameters.AddWithValue("@id", entidad.IdReclutador);

                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0)
                    {
                        return new Response<Reclutador>(true, "Reclutador actualizado correctamente", entidad, null);
                    }
                    return new Response<Reclutador>(false, "No se encontró el reclutador", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error al actualizar: {ex.Message}", null, null);
            }
        }

        // ==========================
        // ELIMINAR (Desactivar Usuario)
        // ==========================
        public Response<Reclutador> Eliminar(int id)
        {
            try
            {
                // En realidad deberíamos desactivar el Usuario, pero si la lógica es borrar de la tabla hija:
                string sentencia = "DELETE FROM [Reclutador] WHERE [idReclutador] = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    int filas = comando.ExecuteNonQuery();

                    if (filas > 0) return new Response<Reclutador>(true, "Reclutador eliminado", null, null);
                    return new Response<Reclutador>(false, "No se encontró el reclutador", null, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error al eliminar: {ex.Message}", null, null);
            }
        }

        // ==========================
        // OBTENER POR ID
        // ==========================
        public Response<Reclutador> ObtenerPorId(int id)
        {
            try
            {
                // 🚨 CORRECCIÓN: Nombres de columnas (contrasena, idEmpresa)
                string sentencia = @"
                    SELECT r.idReclutador, u.nombre, u.correo, u.contrasena, u.estado, 
                           r.cargo, r.idEmpresa
                    FROM Usuario u
                    INNER JOIN Reclutador r ON u.idUsuario = r.idReclutador
                    WHERE r.idReclutador = @id";

                using (SqlConnection conexion = CrearConexion())
                using (SqlCommand comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var reclutador = new Reclutador
                            {
                                IdReclutador = reader.GetInt32(0),
                                IdUsuario = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Correo = reader.GetString(2),
                                Contrasena = reader.GetString(3),
                                Estado = reader.GetString(4),
                                Cargo = reader.GetString(5),
                                IdEmpresa = reader.GetInt32(6)
                            };
                            return new Response<Reclutador>(true, "Reclutador encontrado", reclutador, null);
                        }
                        return new Response<Reclutador>(false, "No se encontró el reclutador", null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error al consultar: {ex.Message}", null, null);
            }
        }

        // ==========================
        // OBTENER TODOS
        // ==========================
        public Response<Reclutador> ObtenerTodos()
        {
            try
            {
                var lista = new List<Reclutador>();
                // 🚨 CORRECCIÓN: Nombres de columnas
                string sentencia = @"
                    SELECT r.idReclutador, u.nombre, u.correo, u.contrasena, u.estado, 
                           r.cargo, r.idEmpresa
                    FROM Usuario u
                    INNER JOIN Reclutador r ON u.idUsuario = r.idReclutador
                    ORDER BY u.nombre";

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
                                IdReclutador = reader.GetInt32(0),
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
                return new Response<Reclutador>(true, $"Encontrados {lista.Count}", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Reclutador>(false, $"Error al obtener todos: {ex.Message}", null, null);
            }
        }
    }
}
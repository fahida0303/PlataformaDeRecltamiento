using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ENTITY;

namespace DAL
{
    public class ConvocatoriaRepository : BaseRepository, IRepository<Convocatoria>
    {
        // OBTENER TODAS
        public Response<Convocatoria> ObtenerTodos()
        {
            try
            {
                var lista = new List<Convocatoria>();

                // ✅ Usamos los nombres EXACTOS de tu base de datos
                string sentencia = @"
                    SELECT idConvocatoria, titulo, descripcion,
                           fechaPublicacion, fechaLimite, estado,
                           idEmpresa, idReclutador
                    FROM Convocatoria
                    ORDER BY fechaPublicacion DESC"; // Ordenamos por las más recientes

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    conexion.Open();
                    using (var reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(MapearConvocatoria(reader));
                        }
                    }
                }

                return new Response<Convocatoria>(true, $"Se encontraron {lista.Count} convocatorias", null, lista);
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error en repositorio: {ex.Message}", null, null);
            }
        }

        // OBTENER POR ID
        public Response<Convocatoria> ObtenerPorId(int id)
        {
            try
            {
                string sentencia = @"
                    SELECT idConvocatoria, titulo, descripcion,
                           fechaPublicacion, fechaLimite, estado,
                           idEmpresa, idReclutador
                    FROM Convocatoria
                    WHERE idConvocatoria = @id";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();

                    using (var reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Response<Convocatoria>(true, "Encontrada", MapearConvocatoria(reader), null);
                        }
                    }
                }
                return new Response<Convocatoria>(false, "No encontrada", null, null);
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error: {ex.Message}", null, null);
            }
        }

        // INSERTAR (Asegurando nombres de columnas)
        public Response<Convocatoria> Insertar(Convocatoria entidad)
        {
            try
            {
                string sentencia = @"
                    INSERT INTO Convocatoria (titulo, descripcion, fechaPublicacion, fechaLimite, estado, idEmpresa, idReclutador)
                    VALUES (@titulo, @descripcion, @fechaPublicacion, @fechaLimite, @estado, @idEmpresa, @idReclutador);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(sentencia, conexion))
                {
                    comando.Parameters.AddWithValue("@titulo", entidad.Titulo);
                    comando.Parameters.AddWithValue("@descripcion", entidad.Descripcion);
                    comando.Parameters.AddWithValue("@fechaPublicacion", entidad.FechaPublicacion);
                    comando.Parameters.AddWithValue("@fechaLimite", entidad.FechaLimite);
                    comando.Parameters.AddWithValue("@estado", entidad.Estado ?? "Abierta");
                    comando.Parameters.AddWithValue("@idEmpresa", entidad.IdEmpresa);
                    comando.Parameters.AddWithValue("@idReclutador", entidad.IdReclutador);

                    conexion.Open();
                    entidad.IdConvocatoria = (int)comando.ExecuteScalar();
                    return new Response<Convocatoria>(true, "Creada correctamente", entidad, null);
                }
            }
            catch (Exception ex)
            {
                return new Response<Convocatoria>(false, $"Error al crear: {ex.Message}", null, null);
            }
        }

        // ACTUALIZAR y ELIMINAR (Implementación básica requerida por la interfaz)
        public Response<Convocatoria> Actualizar(Convocatoria entidad) { return new Response<Convocatoria>(false, "No implementado", null, null); }
        public Response<Convocatoria> Eliminar(int id) { return new Response<Convocatoria>(false, "No implementado", null, null); }

        // 🔹 MAPEADOR ROBUSTO (Evita errores de nombres)
        private Convocatoria MapearConvocatoria(SqlDataReader reader)
        {
            return new Convocatoria
            {
                IdConvocatoria = reader.GetInt32(reader.GetOrdinal("idConvocatoria")),
                Titulo = reader.GetString(reader.GetOrdinal("titulo")),
                Descripcion = reader.GetString(reader.GetOrdinal("descripcion")),
                FechaPublicacion = reader.GetDateTime(reader.GetOrdinal("fechaPublicacion")),
                FechaLimite = reader.GetDateTime(reader.GetOrdinal("fechaLimite")),
                Estado = reader.GetString(reader.GetOrdinal("estado")),
                IdEmpresa = reader.GetInt32(reader.GetOrdinal("idEmpresa")),
                IdReclutador = reader.GetInt32(reader.GetOrdinal("idReclutador"))
            };
        }
    }
}
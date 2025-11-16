using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace JobsyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<NotificacionesController> _logger;

        public NotificacionesController(IConfiguration configuration, ILogger<NotificacionesController> logger)
        {
            _connectionString = configuration.GetConnectionString("JobsyDB");
            _logger = logger;
        }

        /// <summary>
        /// 📧 ENDPOINT PRINCIPAL PARA N8N
        /// Obtiene datos del candidato y postulación para enviar notificación
        /// URL: POST /api/Notificaciones/enviar-resultado
        /// Body: { "idCandidato": 5, "idPostulacion": 10 }
        /// </summary>
        [HttpPost("enviar-resultado")]
        public IActionResult EnviarResultado([FromBody] NotificacionDTO datos)
        {
            _logger.LogInformation("📧 Preparando notificación - Candidato: {IdCandidato}, Postulación: {IdPostulacion}",
                datos?.IdCandidato, datos?.IdPostulacion);

            try
            {
                // ✅ Validación de datos
                if (datos == null || datos.IdCandidato <= 0 || datos.IdPostulacion <= 0)
                {
                    _logger.LogWarning("⚠️ Datos inválidos recibidos");
                    return BadRequest(new
                    {
                        exito = false,
                        mensaje = "Los campos idCandidato e idPostulacion son obligatorios"
                    });
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 🔍 Query optimizada con JOIN para obtener todos los datos necesarios
                    string query = @"
                        SELECT 
                            u.nombre,
                            u.correo,
                            c.titulo AS cargo,
                            p.estado,
                            p.score,
                            c.descripcion
                        FROM Postulacion p
                        INNER JOIN Usuario u ON p.idCandidato = u.idUsuario
                        INNER JOIN Convocatoria c ON p.idConvocatoria = c.idConvocatoria
                        WHERE p.idPostulacion = @idPostulacion 
                          AND p.idCandidato = @idCandidato";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idPostulacion", datos.IdPostulacion);
                        cmd.Parameters.AddWithValue("@idCandidato", datos.IdCandidato);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string nombre = reader.GetString(0);
                                string correo = reader.GetString(1);
                                string cargo = reader.GetString(2);
                                string estado = reader.GetString(3);
                                int? score = reader.IsDBNull(4) ? null : (int?)reader.GetInt32(4);
                                string descripcion = reader.IsDBNull(5) ? "" : reader.GetString(5);

                                _logger.LogInformation("✓ Datos obtenidos - Candidato: {Nombre}, Estado: {Estado}",
                                    nombre, estado);

                                // 📊 Registrar notificación en BD
                                reader.Close();
                                RegistrarNotificacion(conn, datos.IdCandidato, estado, cargo);

                                // 📧 Preparar datos para n8n
                                return Ok(new
                                {
                                    exito = true,
                                    datosParaEmail = new
                                    {
                                        para = correo,
                                        nombre = nombre,
                                        cargo = cargo,
                                        estado = estado,
                                        score = score,
                                        descripcion = descripcion,
                                        asunto = estado == "Seleccionado"
                                            ? $"¡Felicidades! Has sido seleccionado para {cargo}"
                                            : $"Actualización de tu postulación a {cargo}"
                                    },
                                    mensaje = "Datos preparados para envío de email"
                                });
                            }
                            else
                            {
                                _logger.LogWarning("⚠️ No se encontró la postulación: {Id}", datos.IdPostulacion);
                                return NotFound(new
                                {
                                    exito = false,
                                    mensaje = "No se encontró la postulación o candidato especificado"
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "❌ Error SQL al preparar notificación");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error de base de datos al preparar notificación",
                    error = sqlEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inesperado al preparar notificación");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error inesperado al preparar notificación",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 📋 Obtener notificaciones pendientes de envío
        /// URL: GET /api/Notificaciones/pendientes
        /// </summary>
        [HttpGet("pendientes")]
        public IActionResult ObtenerNotificacionesPendientes()
        {
            _logger.LogInformation("📋 Consultando notificaciones pendientes");

            try
            {
                var notificaciones = new List<object>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            n.idNotificacion,
                            u.nombre,
                            u.correo,
                            n.tipoNotificacion,
                            n.mensaje,
                            n.fechaEnvio
                        FROM NotificacionBot n
                        INNER JOIN Usuario u ON n.idUsuario = u.idUsuario
                        WHERE n.estadoEnvio = 'pendiente'
                        ORDER BY n.fechaEnvio DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                notificaciones.Add(new
                                {
                                    idNotificacion = reader.GetInt32(0),
                                    nombre = reader.GetString(1),
                                    correo = reader.GetString(2),
                                    tipo = reader.GetString(3),
                                    mensaje = reader.GetString(4),
                                    fecha = reader.GetDateTime(5).ToString("yyyy-MM-dd HH:mm")
                                });
                            }
                        }
                    }
                }

                _logger.LogInformation("✓ Se encontraron {Count} notificaciones pendientes", notificaciones.Count);

                return Ok(new
                {
                    exito = true,
                    total = notificaciones.Count,
                    notificaciones = notificaciones
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener notificaciones");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al obtener notificaciones",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// ✅ Marcar notificación como enviada
        /// URL: PUT /api/Notificaciones/5/marcar-enviada
        /// </summary>
        [HttpPut("{id}/marcar-enviada")]
        public IActionResult MarcarNotificacionEnviada(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { exito = false, mensaje = "ID inválido" });
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        UPDATE NotificacionBot 
                        SET estadoEnvio = 'enviado'
                        WHERE idNotificacion = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            _logger.LogInformation("✓ Notificación {Id} marcada como enviada", id);
                            return Ok(new
                            {
                                exito = true,
                                mensaje = "Notificación marcada como enviada"
                            });
                        }
                        else
                        {
                            return NotFound(new
                            {
                                exito = false,
                                mensaje = "Notificación no encontrada"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al marcar notificación");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al marcar notificación",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 🔍 Método auxiliar para registrar notificación en BD
        /// </summary>
        private void RegistrarNotificacion(SqlConnection conn, int idUsuario, string estado, string cargo)
        {
            try
            {
                string tipoNotificacion = estado == "Seleccionado" ? "seleccion" : "rechazo";
                string mensaje = estado == "Seleccionado"
                    ? $"Has sido seleccionado para {cargo}"
                    : $"Tu postulación a {cargo} ha sido revisada";

                string query = @"
                    INSERT INTO NotificacionBot (idUsuario, tipoNotificacion, mensaje, estadoEnvio, fechaEnvio)
                    VALUES (@idUsuario, @tipo, @mensaje, 'enviado', GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@tipo", tipoNotificacion);
                    cmd.Parameters.AddWithValue("@mensaje", mensaje);
                    cmd.ExecuteNonQuery();

                    _logger.LogInformation("✓ Notificación registrada en BD para usuario {Id}", idUsuario);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ No se pudo registrar notificación en BD");
                // No lanzamos excepción para no interrumpir el flujo principal
            }
        }

        /// <summary>
        /// 🏥 Health check del servicio
        /// URL: GET /api/Notificaciones/health
        /// </summary>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    return Ok(new
                    {
                        status = "healthy",
                        database = "connected",
                        servicio = "NotificacionesController",
                        timestamp = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "unhealthy",
                    database = "disconnected",
                    error = ex.Message
                });
            }
        }
    }

    /// <summary>
    /// 📦 DTO para recibir datos desde n8n o desde tu app Jobsy
    /// </summary>
    public class NotificacionDTO
    {
        public int IdCandidato { get; set; }
        public int IdPostulacion { get; set; }
    }
}
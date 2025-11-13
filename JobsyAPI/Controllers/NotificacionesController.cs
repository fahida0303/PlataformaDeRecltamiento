using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace JobsyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionesController : ControllerBase
    {
        private readonly string _connectionString;

        public NotificacionesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("JobsyDB");
        }

    
        // URL: POST /api/notificaciones/enviar-resultado
        // Body: { "idCandidato": 5, "idPostulacion": 10 }
  
        [HttpPost("enviar-resultado")]
        public IActionResult EnviarResultado([FromBody] NotificacionDTO datos)
        {
            try
            {
                
                if (datos == null || datos.IdCandidato <= 0 || datos.IdPostulacion <= 0)
                {
                    return BadRequest(new
                    {
                        exito = false,
                        mensaje = "Datos inválidos"
                    });
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            u.nombre,
                            u.correo,
                            c.titulo,
                            p.estado
                        FROM Postulacion p
                        INNER JOIN Usuario u ON p.idCandidato = u.idUsuario
                        INNER JOIN Convocatoria c ON p.idConvocatoria = c.idConvocatoria
                        WHERE p.idPostulacion = @idPostulacion";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idPostulacion", datos.IdPostulacion);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string nombre = reader.GetString(0);
                                string correo = reader.GetString(1);
                                string cargo = reader.GetString(2);
                                string estado = reader.GetString(3);

                         
                                return Ok(new
                                {
                                    exito = true,
                                    datosParaEmail = new
                                    {
                                        para = correo,
                                        nombre = nombre,
                                        cargo = cargo,
                                        estado = estado,
                                        asunto = estado == "Seleccionado"
                                            ? "¡Felicitaciones! Has sido seleccionado"
                                            : "Resultado de tu postulación"
                                    }
                                });
                            }
                            else
                            {
                                return NotFound(new
                                {
                                    exito = false,
                                    mensaje = "Postulación no encontrada"
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al preparar notificación",
                    error = ex.Message
                });
            }
        }

  
        // URL: GET /api/notificaciones/pendientes
        [HttpGet("pendientes")]
        public IActionResult ObtenerNotificacionesPendientes()
        {
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

                return Ok(new
                {
                    exito = true,
                    total = notificaciones.Count,
                    notificaciones = notificaciones
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al obtener notificaciones",
                    error = ex.Message
                });
            }
        }
    }

    public class NotificacionDTO
    {
        public int IdCandidato { get; set; }
        public int IdPostulacion { get; set; }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace JobsyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalendarioController : ControllerBase
    {
        private readonly string _connectionString;

        public CalendarioController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("JobsyDB");
        }

      
        // URL: POST /api/calendario/agendar-entrevista
        // Body: { "idCandidato": 5, "idReclutador": 2, "fechaHora": "2025-02-15T10:00:00", "enlaceMeet": "meet.google.com/abc" }
        [HttpPost("agendar-entrevista")]
        public IActionResult AgendarEntrevista([FromBody] EntrevistaDTO datos)
        {
            try
            {
                
                if (datos == null || datos.IdCandidato <= 0 || datos.IdReclutador <= 0)
                {
                    return BadRequest(new { exito = false, mensaje = "Datos inválidos" });
                }

                if (datos.FechaHora <= DateTime.Now)
                {
                    return BadRequest(new
                    {
                        exito = false,
                        mensaje = "La fecha debe ser futura"
                    });
                }

                int idReunion = 0;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

        
                    string query = @"
                        INSERT INTO Reunion (fecha, enlaceMeet, estadoConfirmacion, idCandidato, idReclutador)
                        VALUES (@fecha, @enlace, 'Pendiente', @idCandidato, @idReclutador);
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@fecha", datos.FechaHora);
                        cmd.Parameters.AddWithValue("@enlace", datos.EnlaceMeet ?? "");
                        cmd.Parameters.AddWithValue("@idCandidato", datos.IdCandidato);
                        cmd.Parameters.AddWithValue("@idReclutador", datos.IdReclutador);

                        
                        idReunion = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }

                return Ok(new
                {
                    exito = true,
                    idReunion = idReunion,
                    mensaje = "Entrevista agendada correctamente"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al agendar entrevista",
                    error = ex.Message
                });
            }
        }

       
        // URL: GET /api/calendario/proximas?dias=7
        [HttpGet("proximas")]
        public IActionResult ObtenerEntrevistasProximas([FromQuery] int dias = 7)
        {
            try
            {
                var entrevistas = new List<object>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            r.idReunion,
                            r.fecha,
                            r.enlaceMeet,
                            r.estadoConfirmacion,
                            uc.nombre AS candidato,
                            ur.nombre AS reclutador
                        FROM Reunion r
                        INNER JOIN Usuario uc ON r.idCandidato = uc.idUsuario
                        INNER JOIN Usuario ur ON r.idReclutador = ur.idUsuario
                        WHERE r.fecha BETWEEN GETDATE() AND DATEADD(DAY, @dias, GETDATE())
                        ORDER BY r.fecha ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dias", dias);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entrevistas.Add(new
                                {
                                    idReunion = reader.GetInt32(0),
                                    fecha = reader.GetDateTime(1).ToString("yyyy-MM-dd HH:mm"),
                                    enlaceMeet = reader.GetString(2),
                                    estado = reader.GetString(3),
                                    candidato = reader.GetString(4),
                                    reclutador = reader.GetString(5)
                                });
                            }
                        }
                    }
                }

                return Ok(new
                {
                    exito = true,
                    total = entrevistas.Count,
                    entrevistas = entrevistas
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al obtener entrevistas",
                    error = ex.Message
                });
            }
        }
    }

 
    public class EntrevistaDTO
    {
        public int IdCandidato { get; set; }
        public int IdReclutador { get; set; }
        public DateTime FechaHora { get; set; }
        public string EnlaceMeet { get; set; }
    }
}
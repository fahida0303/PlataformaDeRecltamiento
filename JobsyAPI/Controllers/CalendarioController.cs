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

        // 🟢 MODIFICADO: Ahora acepta idReclutador para filtrar
        [HttpGet("proximas")]
        public IActionResult ObtenerEntrevistasProximas([FromQuery] int dias = 30, [FromQuery] int? idReclutador = null)
        {
            try
            {
                var entrevistas = new List<object>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Base de la consulta
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
                        WHERE r.fecha BETWEEN GETDATE() AND DATEADD(DAY, @dias, GETDATE())";

                    // 🟢 Si nos envían un ID de reclutador, filtramos por él
                    if (idReclutador.HasValue && idReclutador.Value > 0)
                    {
                        query += " AND r.idReclutador = @idReclutador";
                    }

                    query += " ORDER BY r.fecha ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dias", dias);

                        if (idReclutador.HasValue && idReclutador.Value > 0)
                        {
                            cmd.Parameters.AddWithValue("@idReclutador", idReclutador.Value);
                        }

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

        [HttpPut("actualizar-reunion/{id}")]
        public async Task<IActionResult> ActualizarReunionConMeet(int id, [FromBody] ActualizarReunionMeetRequest request)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string verificarQuery = "SELECT COUNT(*) FROM Reunion WHERE idReunion = @IdReunion";
                    using (SqlCommand cmdVerificar = new SqlCommand(verificarQuery, conn))
                    {
                        cmdVerificar.Parameters.AddWithValue("@IdReunion", id);
                        int existe = Convert.ToInt32(await cmdVerificar.ExecuteScalarAsync());

                        if (existe == 0)
                        {
                            return NotFound(new
                            {
                                exito = false,
                                mensaje = $"No se encontró una reunión con ID {id}"
                            });
                        }
                    }

                    string updateQuery = @"
                        UPDATE Reunion
                        SET 
                            enlaceMeet = @EnlaceMeet,
                            idEventoCalendar = @IdEventoCalendar,
                            estadoConfirmacion = @EstadoConfirmacion,
                            fechaModificacion = GETDATE()
                        WHERE idReunion = @IdReunion";

                    using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@IdReunion", id);
                        cmdUpdate.Parameters.AddWithValue("@EnlaceMeet", request.EnlaceMeet ?? "");
                        cmdUpdate.Parameters.AddWithValue("@IdEventoCalendar", request.IdEventoCalendar ?? "");
                        cmdUpdate.Parameters.AddWithValue("@EstadoConfirmacion", request.EstadoConfirmacion ?? "Confirmada");

                        await cmdUpdate.ExecuteNonQueryAsync();
                    }

                    return Ok(new
                    {
                        exito = true,
                        mensaje = "Reunión actualizada exitosamente con enlace de Meet",
                        idReunion = id,
                        enlaceMeet = request.EnlaceMeet,
                        idEventoCalendar = request.IdEventoCalendar
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al actualizar la reunión",
                    detalle = ex.Message
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

    public class ActualizarReunionMeetRequest
    {
        public string EnlaceMeet { get; set; }
        public string IdEventoCalendar { get; set; }
        public string EstadoConfirmacion { get; set; }
    }
}
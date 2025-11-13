using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using ENTITY;


namespace JobsyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidatosController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<CandidatosController> _logger;

        public CandidatosController(IConfiguration configuration, ILogger<CandidatosController> logger)
        {
            _connectionString = configuration.GetConnectionString("JobsyDB");
            _logger = logger;
        }


        // POST: api/candidatos/registro
        [HttpPost("registro")]
        public IActionResult RegistrarCandidato([FromBody] RegistroCandidatoDTO dto)
        {
            _logger.LogInformation("📝 Intentando registrar candidato: {Nombre}", dto?.Nombre);

            try
            {

                if (dto == null)
                {
                    return BadRequest(new { exito = false, mensaje = "Datos inválidos" });
                }

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                {
                    return BadRequest(new { exito = false, mensaje = "El nombre es obligatorio" });
                }

                if (string.IsNullOrWhiteSpace(dto.TelegramId))
                {
                    return BadRequest(new { exito = false, mensaje = "El TelegramId es obligatorio" });
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 1. Verificar si el usuario ya existe por TelegramId
                    string queryVerificar = @"
                        SELECT idUsuario, nombre, correo
                        FROM Usuario 
                        WHERE telegramId = @telegramId";

                    using (SqlCommand cmdVerificar = new SqlCommand(queryVerificar, conn))
                    {
                        cmdVerificar.Parameters.AddWithValue("@telegramId", dto.TelegramId);

                        using (SqlDataReader reader = cmdVerificar.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                _logger.LogInformation("✓ Usuario ya registrado: {Id}", reader.GetInt32(0));

                                return Ok(new
                                {
                                    exito = true,
                                    yaRegistrado = true,
                                    idUsuario = reader.GetInt32(0),
                                    nombre = reader.GetString(1),
                                    correo = reader.GetString(2),
                                    mensaje = "¡Ya estás registrado! Bienvenido de nuevo."
                                });
                            }
                        }
                    }


                    string queryUsuario = @"
                        INSERT INTO Usuario 
                        (nombre, correo, contraseña, estado, telegramId, telegramUsername, whatsappNumber, fechaUltimaInteraccionBot)
                        VALUES 
                        (@nombre, @correo, @contrasena, 'Activo', @telegramId, @telegramUsername, @whatsapp, GETDATE());
                        SELECT SCOPE_IDENTITY();";

                    int idUsuario;
                    using (SqlCommand cmdUsuario = new SqlCommand(queryUsuario, conn))
                    {

                        string correo = dto.Correo;
                        if (string.IsNullOrWhiteSpace(correo))
                        {
                            correo = !string.IsNullOrWhiteSpace(dto.TelegramUsername)
                                ? $"{dto.TelegramUsername}@telegram.temp"
                                : $"user_{dto.TelegramId}@telegram.temp";
                        }

                        cmdUsuario.Parameters.AddWithValue("@nombre", dto.Nombre);
                        cmdUsuario.Parameters.AddWithValue("@correo", correo);
                        cmdUsuario.Parameters.AddWithValue("@contrasena", $"telegram_user_{DateTime.Now.Ticks}");
                        cmdUsuario.Parameters.AddWithValue("@telegramId", dto.TelegramId);
                        cmdUsuario.Parameters.AddWithValue("@telegramUsername",
                            dto.TelegramUsername ?? (object)DBNull.Value);
                        cmdUsuario.Parameters.AddWithValue("@whatsapp",
                            dto.WhatsappNumber ?? (object)DBNull.Value);

                        idUsuario = Convert.ToInt32(cmdUsuario.ExecuteScalar());
                        _logger.LogInformation("✓ Usuario creado con ID: {Id}", idUsuario);
                    }

                    // 3. Insertar candidato
                    string queryCandidato = @"
                        INSERT INTO Candidato (idCandidato, tipox, nivelFormacion)
                        VALUES (@idCandidato, 'Externo', NULL)";

                    using (SqlCommand cmdCandidato = new SqlCommand(queryCandidato, conn))
                    {
                        cmdCandidato.Parameters.AddWithValue("@idCandidato", idUsuario);
                        cmdCandidato.ExecuteNonQuery();
                        _logger.LogInformation("✓ Candidato creado con ID: {Id}", idUsuario);
                    }

                    return Ok(new
                    {
                        exito = true,
                        yaRegistrado = false,
                        idUsuario = idUsuario,
                        nombre = dto.Nombre,
                        mensaje = "¡Registro exitoso! Bienvenido a Jobsy."
                    });
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "❌ Error SQL al registrar candidato");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error de base de datos al registrar candidato",
                    error = sqlEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al registrar candidato");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error inesperado al registrar candidato",
                    error = ex.Message
                });
            }
        }


        // GET: api/candidatos/convocatorias
        [HttpGet("convocatorias")]
        public IActionResult ObtenerConvocatorias([FromQuery] string estado = "Activa")
        {
            _logger.LogInformation("📋 Consultando convocatorias con estado: {Estado}", estado);

            try
            {
                List<dynamic> convocatorias = new List<dynamic>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT c.idConvocatoria, c.titulo, c.descripcion, 
                               c.fechaInicio, c.fechaFin, c.estado,
                               COUNT(p.idPostulacion) as totalPostulaciones
                        FROM Convocatoria c
                        LEFT JOIN Postulacion p ON c.idConvocatoria = p.idConvocatoria
                        WHERE c.estado = @estado
                        GROUP BY c.idConvocatoria, c.titulo, c.descripcion, 
                                 c.fechaInicio, c.fechaFin, c.estado
                        ORDER BY c.fechaInicio DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@estado", estado);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                convocatorias.Add(new
                                {
                                    id = reader.GetInt32(0),
                                    titulo = reader.GetString(1),
                                    descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    fechaInicio = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    fechaFin = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    estado = reader.GetString(5),
                                    totalPostulaciones = reader.GetInt32(6)
                                });
                            }
                        }
                    }
                }

                _logger.LogInformation("✓ Se encontraron {Count} convocatorias", convocatorias.Count);

                return Ok(new
                {
                    exito = true,
                    total = convocatorias.Count,
                    convocatorias = convocatorias
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener convocatorias");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al obtener convocatorias",
                    error = ex.Message
                });
            }
        }


        // GET: api/candidatos/estado/{telegramId}
        [HttpGet("estado/{telegramId}")]
        public IActionResult ObtenerEstadoCandidato(string telegramId)
        {
            _logger.LogInformation("🔍 Consultando estado para TelegramId: {Id}", telegramId);

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();


                    string queryUsuario = @"
                        SELECT u.idUsuario, u.nombre, u.correo, 
                               c.nivelFormacion, c.experiencia
                        FROM Usuario u
                        INNER JOIN Candidato c ON u.idUsuario = c.idCandidato
                        WHERE u.telegramId = @telegramId";

                    using (SqlCommand cmd = new SqlCommand(queryUsuario, conn))
                    {
                        cmd.Parameters.AddWithValue("@telegramId", telegramId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                _logger.LogWarning("⚠️ Candidato no encontrado: {Id}", telegramId);
                                return NotFound(new
                                {
                                    encontrado = false,
                                    mensaje = "No estás registrado. Usa /start para registrarte."
                                });
                            }

                            int idUsuario = reader.GetInt32(0);
                            var candidato = new
                            {
                                id = idUsuario,
                                nombre = reader.GetString(1),
                                correo = reader.GetString(2),
                                nivelFormacion = reader.IsDBNull(3) ? "No especificado" : reader.GetString(3),
                                experiencia = reader.IsDBNull(4) ? "No especificada" : reader.GetString(4)
                            };

                            reader.Close();

                            List<dynamic> postulaciones = new List<dynamic>();
                            string queryPostulaciones = @"
                                SELECT p.idPostulacion, p.fechaPostulacion, p.estado,
                                       c.titulo, c.descripcion
                                FROM Postulacion p
                                INNER JOIN Convocatoria c ON p.idConvocatoria = c.idConvocatoria
                                WHERE p.idCandidato = @idUsuario
                                ORDER BY p.fechaPostulacion DESC";

                            using (SqlCommand cmdPost = new SqlCommand(queryPostulaciones, conn))
                            {
                                cmdPost.Parameters.AddWithValue("@idUsuario", idUsuario);

                                using (SqlDataReader readerPost = cmdPost.ExecuteReader())
                                {
                                    while (readerPost.Read())
                                    {
                                        postulaciones.Add(new
                                        {
                                            id = readerPost.GetInt32(0),
                                            fecha = readerPost.GetDateTime(1).ToString("yyyy-MM-dd"),
                                            estado = readerPost.GetString(2),
                                            convocatoria = readerPost.GetString(3),
                                            descripcion = readerPost.IsDBNull(4) ? "" : readerPost.GetString(4)
                                        });
                                    }
                                }
                            }

                            _logger.LogInformation("✓ Estado consultado para: {Nombre}", candidato.nombre);

                            return Ok(new
                            {
                                encontrado = true,
                                candidato = candidato,
                                postulaciones = postulaciones,
                                totalPostulaciones = postulaciones.Count
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al consultar estado");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al consultar estado",
                    error = ex.Message
                });
            }
        }


        // POST: api/candidatos/postular
        [HttpPost("postular")]
        public IActionResult PostularAConvocatoria([FromBody] PostulacionDTO dto)
        {
            _logger.LogInformation("📬 Postulación: TelegramId {Id} a Convocatoria {Conv}",
                dto?.TelegramId, dto?.IdConvocatoria);

            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.TelegramId) || dto.IdConvocatoria <= 0)
                {
                    return BadRequest(new { exito = false, mensaje = "Datos inválidos" });
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();


                    string queryUsuario = "SELECT idUsuario FROM Usuario WHERE telegramId = @telegramId";
                    int idUsuario;

                    using (SqlCommand cmdUsuario = new SqlCommand(queryUsuario, conn))
                    {
                        cmdUsuario.Parameters.AddWithValue("@telegramId", dto.TelegramId);
                        var result = cmdUsuario.ExecuteScalar();

                        if (result == null)
                        {
                            return NotFound(new { exito = false, mensaje = "Usuario no registrado" });
                        }

                        idUsuario = Convert.ToInt32(result);
                    }

                    string queryVerificar = @"
                        SELECT COUNT(*) 
                        FROM Postulacion 
                        WHERE idCandidato = @idUsuario AND idConvocatoria = @idConvocatoria";

                    using (SqlCommand cmdVerificar = new SqlCommand(queryVerificar, conn))
                    {
                        cmdVerificar.Parameters.AddWithValue("@idUsuario", idUsuario);
                        cmdVerificar.Parameters.AddWithValue("@idConvocatoria", dto.IdConvocatoria);

                        int count = Convert.ToInt32(cmdVerificar.ExecuteScalar());
                        if (count > 0)
                        {
                            return Ok(new
                            {
                                exito = false,
                                yaPostulado = true,
                                mensaje = "Ya te postulaste a esta convocatoria anteriormente."
                            });
                        }
                    }

             
                    string queryPostular = @"
                        INSERT INTO Postulacion (idCandidato, idConvocatoria, fechaPostulacion, estado)
                        VALUES (@idUsuario, @idConvocatoria, GETDATE(), 'Pendiente');
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmdPostular = new SqlCommand(queryPostular, conn))
                    {
                        cmdPostular.Parameters.AddWithValue("@idUsuario", idUsuario);
                        cmdPostular.Parameters.AddWithValue("@idConvocatoria", dto.IdConvocatoria);

                        int idPostulacion = Convert.ToInt32(cmdPostular.ExecuteScalar());

                        _logger.LogInformation("✓ Postulación creada con ID: {Id}", idPostulacion);

                        return Ok(new
                        {
                            exito = true,
                            idPostulacion = idPostulacion,
                            mensaje = "¡Postulación exitosa! Te notificaremos sobre el proceso."
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al postular");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al procesar postulación",
                    error = ex.Message
                });
            }
        }


        // GET: api/candidatos/health
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


        // URL: PUT /api/candidatos/10/score
        [HttpPut("{id}/score")]
        public IActionResult ActualizarScore(int id, [FromBody] ScoreDTO datos)
        {
            try
            {

                if (datos == null || datos.Score < 0 || datos.Score > 100)
                {
                    return BadRequest(new
                    {
                        exito = false,
                        mensaje = "El score debe estar entre 0 y 100"
                    });
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                UPDATE Postulacion 
                SET score = @score, 
                    estado = 'Evaluado'
                WHERE idPostulacion = @idPostulacion";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@score", datos.Score);
                        cmd.Parameters.AddWithValue("@idPostulacion", id);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            return Ok(new
                            {
                                exito = true,
                                idPostulacion = id,
                                score = datos.Score,
                                mensaje = "Score actualizado correctamente"
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al actualizar score",
                    error = ex.Message
                });
            }
        }


        public class ScoreDTO
        {
            public int Score { get; set; }
        }
    }

   

    public class RegistroCandidatoDTO
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string TelegramId { get; set; }
        public string TelegramUsername { get; set; }
        public string WhatsappNumber { get; set; }
    }

    public class PostulacionDTO
    {
        public  string TelegramId { get; set; }
        public int IdConvocatoria { get; set; }
    }
}
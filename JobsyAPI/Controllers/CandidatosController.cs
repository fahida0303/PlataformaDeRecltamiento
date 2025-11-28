using BLL;
using ENTITY;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace JobsyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidatosController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly ILogger<CandidatosController> _logger;
        private readonly string _n8nWebhookUrl;

        public CandidatosController(IConfiguration configuration, ILogger<CandidatosController> logger)
        {
            _connectionString = configuration.GetConnectionString("JobsyDB");
            _n8nWebhookUrl = configuration["N8N:NotificacionesWebhook"];
            _logger = logger;
        }

        #region 🔹 CONSULTAS DE CONVOCATORIAS

        
        [HttpGet("convocatorias")]
        public IActionResult ObtenerConvocatorias([FromQuery] string estado = "Abierta")
        {
            try
            {
                List<dynamic> convocatorias = new List<dynamic>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT c.idConvocatoria, c.titulo, c.descripcion,
                            c.fechaPublicacion, c.fechaLimite, c.estado,
                            (SELECT COUNT(*) FROM Postulacion p 
                             WHERE p.idConvocatoria = c.idConvocatoria) as totalPostulaciones
                        FROM Convocatoria c
                        ORDER BY c.idConvocatoria DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            convocatorias.Add(new
                            {
                                id = reader.GetInt32(0),
                                titulo = reader.GetString(1),
                                descripcion = reader.IsDBNull(2) ? "Sin descripción" : reader.GetString(2),
                                fechaInicio = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                fechaFin = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                estado = reader.GetString(5),
                                totalPostulaciones = reader.GetInt32(6)
                            });
                        }
                    }
                }

                return Ok(new { exito = true, total = convocatorias.Count, convocatorias });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error interno", error = ex.Message });
            }
        }

        
        [HttpGet("reclutador/{idReclutador}/convocatorias")]
        public IActionResult ObtenerConvocatoriasPorReclutador(int idReclutador)
        {
            try
            {
                List<dynamic> convocatorias = new List<dynamic>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT c.idConvocatoria, c.titulo, c.descripcion,
                            c.fechaPublicacion, c.fechaLimite, c.estado,
                            (SELECT COUNT(*) FROM Postulacion p 
                             WHERE p.idConvocatoria = c.idConvocatoria) as totalPostulaciones
                        FROM Convocatoria c
                        WHERE c.idReclutador = @id
                        ORDER BY c.fechaPublicacion DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idReclutador);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                convocatorias.Add(new
                                {
                                    id = reader.GetInt32(0),
                                    titulo = reader.GetString(1),
                                    descripcion = reader.IsDBNull(2) ? "Sin descripción" : reader.GetString(2),
                                    fechaInicio = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    fechaFin = reader.GetDateTime(4).ToString("yyyy-MM-dd"),
                                    estado = reader.GetString(5),
                                    totalPostulaciones = reader.GetInt32(6)
                                });
                            }
                        }
                    }
                }

                return Ok(new { exito = true, total = convocatorias.Count, convocatorias });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error interno", error = ex.Message });
            }
        }

        #endregion

        #region 🔹 PROCESOS AUTOMÁTICOS

        
        [HttpGet("vencidas")]
        public IActionResult ObtenerConvocatoriasVencidas()
        {
            try
            {
                List<dynamic> convocatorias = new List<dynamic>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT idConvocatoria, titulo, descripcion, fechaLimite 
                        FROM Convocatoria 
                        WHERE fechaLimite < GETDATE() AND estado = 'Abierta'
                        ORDER BY fechaLimite DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            convocatorias.Add(new
                            {
                                idConvocatoria = reader.GetInt32(0),
                                titulo = reader.GetString(1),
                                descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                fechaLimite = reader.GetDateTime(3).ToString("yyyy-MM-dd")
                            });
                        }
                    }
                }

                return Ok(new { total = convocatorias.Count, convocatorias });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error", error = ex.Message });
            }
        }

        
        [HttpPut("{idConvocatoria}/cerrar")]
        public IActionResult CerrarConvocatoria(int idConvocatoria)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = "UPDATE Convocatoria SET estado = 'Cerrada' WHERE idConvocatoria = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idConvocatoria);

                        int filas = cmd.ExecuteNonQuery();

                        if (filas > 0)
                            return Ok(new { exito = true, mensaje = "Convocatoria cerrada" });

                        return NotFound(new { exito = false, mensaje = "Convocatoria no encontrada" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error", error = ex.Message });
            }
        }

       
        [HttpGet("{idConvocatoria}/top-candidatos")]
        public IActionResult ObtenerTopCandidatos(int idConvocatoria)
        {
            try
            {
                List<dynamic> candidatos = new List<dynamic>();
                string emailReclutador = "";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Email del reclutador
                    string queryReclutador = @"
                        SELECT u.correo 
                        FROM Convocatoria c 
                        INNER JOIN Usuario u ON c.idReclutador = u.idUsuario
                        WHERE c.idConvocatoria = @id";

                    using (SqlCommand cmdRec = new SqlCommand(queryReclutador, conn))
                    {
                        cmdRec.Parameters.AddWithValue("@id", idConvocatoria);
                        emailReclutador = cmdRec.ExecuteScalar()?.ToString() ?? "jobsyapp1@gmail.com";
                    }

                    // Top candidatos
                    string query = @"
                        SELECT TOP 3 u.nombre, u.correo, p.score
                        FROM Postulacion p
                        INNER JOIN Candidato c ON p.idCandidato = c.idCandidato
                        INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                        WHERE p.idConvocatoria = @id AND p.score IS NOT NULL
                        ORDER BY p.score DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idConvocatoria);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                candidatos.Add(new
                                {
                                    nombre = reader.GetString(0),
                                    correo = reader.GetString(1),
                                    score = Math.Round(reader.GetDecimal(2), 1)
                                });
                            }
                        }
                    }
                }

                return Ok(new { candidatos, emailReclutador });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error", error = ex.Message });
            }
        }

        #endregion

        #region 📄 GESTIÓN DE CVs & PERFIL

        // ACTUALIZAR FOTO DE PERFIL
        [HttpPost("{idUsuario}/foto")]
        public async Task<IActionResult> ActualizarFoto(int idUsuario, IFormFile archivoFoto)
        {
            try
            {
                if (archivoFoto == null || archivoFoto.Length == 0)
                    return BadRequest(new { exito = false, mensaje = "No se recibió ninguna imagen" });

                byte[] fotoBytes;
                using (var ms = new MemoryStream())
                {
                    await archivoFoto.CopyToAsync(ms);
                    fotoBytes = ms.ToArray();
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    // Actualizamos la tabla USUARIO, ya que ahí está el campo foto
                    string query = "UPDATE Usuario SET foto = @foto WHERE idUsuario = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idUsuario);
                        // Especificamos que es VarBinary
                        var param = new SqlParameter("@foto", System.Data.SqlDbType.VarBinary);
                        param.Value = fotoBytes;
                        cmd.Parameters.Add(param);

                        int filas = cmd.ExecuteNonQuery();
                        if (filas > 0)
                            return Ok(new { exito = true, mensaje = "Foto actualizada" });

                        return NotFound(new { exito = false, mensaje = "Usuario no encontrado" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", error = ex.Message });
            }
        }

        // SUBIR PDF CV
        [HttpPost("{idCandidato}/subir-cv")]
        public async Task<IActionResult> SubirCV(int idCandidato, IFormFile archivoPdf)
        {
            try
            {
                if (archivoPdf == null || archivoPdf.Length == 0)
                    return BadRequest(new { exito = false, mensaje = "No archivo" });

                byte[] pdfBytes;

                using (var ms = new MemoryStream())
                {
                    await archivoPdf.CopyToAsync(ms);
                    pdfBytes = ms.ToArray();
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string queryUpdate = "UPDATE Candidato SET hojaDeVida = @pdf WHERE idCandidato = @id";

                    using (SqlCommand cmd = new SqlCommand(queryUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idCandidato);
                        cmd.Parameters.Add("@pdf", System.Data.SqlDbType.VarBinary).Value = pdfBytes;

                        int filas = cmd.ExecuteNonQuery();

                        if (filas > 0)
                            return Ok(new { exito = true, mensaje = "CV subido" });

                        return StatusCode(500, new { exito = false, mensaje = "No se actualizó" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error", error = ex.Message });
            }
        }

        // DESCARGAR CV
        [HttpGet("{idCandidato}/descargar-cv")]
        public IActionResult DescargarCV(int idCandidato)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT u.nombre, c.hojaDeVida 
                        FROM Candidato c 
                        JOIN Usuario u ON c.idCandidato = u.idUsuario
                        WHERE c.idCandidato = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idCandidato);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.IsDBNull(1))
                                    return NotFound(new { exito = false, mensaje = "Sin CV" });

                                byte[] pdfBytes = (byte[])reader["hojaDeVida"];
                                return File(pdfBytes, "application/pdf", $"CV.pdf");
                            }

                            return NotFound(new { exito = false, mensaje = "Candidato no encontrado" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error", error = ex.Message });
            }
        }

        // OBTENER CANDIDATOS CON CV
        [HttpGet("convocatoria/{idConvocatoria}/candidatos")]
        public IActionResult ObtenerCandidatosConCV(int idConvocatoria)
        {
            try
            {
                var candidatos = new List<object>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT p.idPostulacion, p.idCandidato, u.nombre, u.correo,
                               c.hojaDeVida, p.score, p.estado
                        FROM Postulacion p
                        INNER JOIN Candidato c ON p.idCandidato = c.idCandidato
                        INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                        WHERE p.idConvocatoria = @id
                        ORDER BY p.score DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idConvocatoria);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                byte[] pdfBytes = reader.IsDBNull(4) ? null : (byte[])reader["hojaDeVida"];
                                string textoCV = "Sin CV disponible";

                                if (pdfBytes != null && pdfBytes.Length > 0)
                                {
                                    try
                                    {
                                        textoCV = PdfService.ExtraerTextoDePdf(pdfBytes);
                                    }
                                    catch
                                    {
                                        textoCV = "Error lectura PDF";
                                    }
                                }

                                candidatos.Add(new
                                {
                                    idPostulacion = reader.GetInt32(0),
                                    idCandidato = reader.GetInt32(1),
                                    nombre = reader.GetString(2),
                                    correo = reader.GetString(3),
                                    hojaDeVida = textoCV,
                                    score = reader.IsDBNull(5) ? null : (decimal?)reader.GetDecimal(5),
                                    estado = reader.GetString(6)
                                });
                            }
                        }
                    }
                }

                return Ok(new { idConvocatoria, candidatos });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error", error = ex.Message });
            }
        }

        // 🔥 METODO CORREGIDO 🔥
        // OBTENER ESTADO DE UN CANDIDATO (O RECLUTADOR)
        [HttpGet("estado/{identificador}")]
        public IActionResult ObtenerEstadoCandidato(string identificador)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 1. OBTENER INFO DE USUARIO (USANDO LEFT JOIN PARA QUE FUNCIONE CON RECLUTADORES)
                    string queryUsuario = @"
                        SELECT u.idUsuario, u.nombre, u.correo,
                               c.nivelFormacion, c.experiencia,
                               u.whatsappNumber, u.documento,
                               u.fechaNacimiento, u.foto
                        FROM Usuario u
                        LEFT JOIN Candidato c ON u.idUsuario = c.idCandidato 
                        WHERE u.telegramId = @id OR CAST(u.idUsuario AS VARCHAR) = @id";

                    using (SqlCommand cmd = new SqlCommand(queryUsuario, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", identificador);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                                return NotFound(new { encontrado = false });

                            byte[] fotoBytes = reader.IsDBNull(8) ? null : (byte[])reader["foto"];

                            string fotoBase64 = fotoBytes != null
                                ? "data:image/jpeg;base64," + Convert.ToBase64String(fotoBytes)
                                : null;

                            // 🔥 OBJETO CORREGIDO PARA COINCIDIR CON REACT
                            var candidato = new
                            {
                                id = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                correo = reader.GetString(2),
                                // Usamos IsDBNull para manejar el caso de Reclutadores (que no tienen estos campos)
                                nivelFormacion = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                experiencia = reader.IsDBNull(4) ? "" : reader.GetString(4),

                                // 🔥 CAMBIO CLAVE: 'telefono' en vez de 'whatsappNumber' para que React lo lea
                                telefono = reader.IsDBNull(5) ? "" : reader.GetString(5),

                                documento = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                fechaNacimiento = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                                foto = fotoBase64
                            };

                            reader.Close();

                            // 2. OBTENER POSTULACIONES
                            List<dynamic> postulaciones = new List<dynamic>();

                            string queryPost = @"
                                SELECT p.idPostulacion, p.fechaPostulacion, p.estado, c.titulo
                                FROM Postulacion p
                                INNER JOIN Convocatoria c ON p.idConvocatoria = c.idConvocatoria
                                WHERE p.idCandidato = @idUsuario
                                ORDER BY p.fechaPostulacion DESC";

                            using (SqlCommand cmdPost = new SqlCommand(queryPost, conn))
                            {
                                cmdPost.Parameters.AddWithValue("@idUsuario", candidato.id);

                                using (SqlDataReader r2 = cmdPost.ExecuteReader())
                                {
                                    while (r2.Read())
                                    {
                                        postulaciones.Add(new
                                        {
                                            id = r2.GetInt32(0),
                                            fecha = r2.GetDateTime(1).ToString("yyyy-MM-dd"),
                                            estado = r2.GetString(2),
                                            convocatoria = r2.GetString(3)
                                        });
                                    }
                                }
                            }

                            return Ok(new
                            {
                                encontrado = true,
                                candidato,
                                postulaciones,
                                totalPostulaciones = postulaciones.Count
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error", error = ex.Message });
            }
        }

        // ACTUALIZAR PERFIL
        [HttpPut("perfil/{id}")]
        public IActionResult ActualizarPerfil(int id, [FromBody] ActualizarPerfilDTO dto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 1. Actualizar tabla Usuario (Datos comunes)
                    string qUser = @"
                        UPDATE Usuario 
                        SET nombre = @nombre, whatsappNumber = @tel, 
                            documento = @doc, fechaNacimiento = @fecha 
                        WHERE idUsuario = @id";

                    using (SqlCommand cmd = new SqlCommand(qUser, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", dto.Nombre);
                        cmd.Parameters.AddWithValue("@tel", dto.Telefono ?? "");
                        cmd.Parameters.AddWithValue("@doc", dto.Documento ?? "");
                        cmd.Parameters.AddWithValue("@fecha", (object)dto.FechaNacimiento ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Actualizar tabla Candidato (Datos específicos)
                    // OJO: Si es reclutador, esto no actualizará nada (0 filas), lo cual es correcto
                    string qCand = @"
                        UPDATE Candidato 
                        SET nivelFormacion = @form, experiencia = @exp 
                        WHERE idCandidato = @id";

                    using (SqlCommand cmd = new SqlCommand(qCand, conn))
                    {
                        cmd.Parameters.AddWithValue("@form", dto.NivelFormacion ?? "");
                        cmd.Parameters.AddWithValue("@exp", dto.Experiencia ?? "");
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                return Ok(new { exito = true, mensaje = "Perfil actualizado" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error", error = ex.Message });
            }
        }

        #endregion

        #region 📝 REGISTRO Y POSTULACIÓN

        // REGISTRO DE CANDIDATO
        [HttpPost("registro")]
        public IActionResult RegistrarCandidato([FromBody] RegistroCandidatoDTO dto)
        {
            return Ok(new { exito = true, mensaje = "Registrado" });
        }

        // POSTULAR A CONVOCATORIA
        [HttpPost("postular")]
        public IActionResult PostularAConvocatoria([FromBody] PostulacionDTO dto)
        {
            try
            {
                if (dto == null || dto.IdConvocatoria <= 0)
                    return BadRequest(new { exito = false, mensaje = "Datos inválidos" });

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    int idUsuario = 0;

                    if (dto.IdCandidato.HasValue && dto.IdCandidato.Value > 0)
                    {
                        idUsuario = dto.IdCandidato.Value;
                    }
                    else if (!string.IsNullOrWhiteSpace(dto.TelegramId))
                    {
                        string q = "SELECT idUsuario FROM Usuario WHERE telegramId = @tid";

                        using (SqlCommand cmd = new SqlCommand(q, conn))
                        {
                            cmd.Parameters.AddWithValue("@tid", dto.TelegramId);

                            var r = cmd.ExecuteScalar();
                            if (r == null) return NotFound(new { exito = false });

                            idUsuario = Convert.ToInt32(r);
                        }
                    }
                    else
                    {
                        return BadRequest(new { exito = false, mensaje = "Falta ID" });
                    }

                    string qCheck = @"
                        SELECT COUNT(*) FROM Postulacion 
                        WHERE idCandidato=@uid AND idConvocatoria=@cid";

                    using (SqlCommand cmd = new SqlCommand(qCheck, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", idUsuario);
                        cmd.Parameters.AddWithValue("@cid", dto.IdConvocatoria);

                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            return Ok(new { exito = false, mensaje = "Ya postulado" });
                    }

                    string qIns = @"
                        INSERT INTO Postulacion (idCandidato, idConvocatoria, fechaPostulacion, estado)
                        VALUES (@uid, @cid, GETDATE(), 'Pendiente');
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(qIns, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", idUsuario);
                        cmd.Parameters.AddWithValue("@cid", dto.IdConvocatoria);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());

                        return Ok(new
                        {
                            exito = true,
                            idPostulacion = id,
                            mensaje = "Postulación exitosa"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error", error = ex.Message });
            }
        }

        // ACTUALIZAR ESTADO DE POSTULACIÓN
        [HttpPut("postulacion/{id}/estado")]
        public IActionResult ActualizarEstadoPostulacion(int id, [FromBody] ActualizarEstadoDTO dto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        UPDATE Postulacion 
                        SET estado = @estado, fechaModificacion = GETDATE() 
                        WHERE idPostulacion = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@estado", dto.Estado);
                        cmd.Parameters.AddWithValue("@id", id);

                        int filas = cmd.ExecuteNonQuery();

                        if (filas > 0)
                            return Ok(new { exito = true, mensaje = $"Estado actualizado a: {dto.Estado}" });

                        return NotFound(new { exito = false, mensaje = "Postulación no encontrada" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al actualizar estado",
                    error = ex.Message
                });
            }
        }

        // ACTUALIZAR SCORE
        [HttpPut("{id}/score")]
        public IActionResult ActualizarScore(int id, [FromBody] ScoreDTO datos)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        UPDATE Postulacion 
                        SET score = @score, fechaModificacion = GETDATE() 
                        WHERE idPostulacion = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@score", datos.Score);
                        cmd.Parameters.AddWithValue("@id", id);

                        int filas = cmd.ExecuteNonQuery();

                        if (filas > 0)
                            return Ok(new
                            {
                                exito = true,
                                mensaje = $"Score actualizado a: {datos.Score}"
                            });

                        return NotFound(new
                        {
                            exito = false,
                            mensaje = "Postulación no encontrada o no actualizada."
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar score de postulación {Id}", id);

                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al actualizar score",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region 📤 NOTIFICACIÓN A n8n

        // NOTIFICAR DECISIÓN (ACEPTAR/RECHAZAR)
        [HttpPost("notificar-decision")]
        public async Task<IActionResult> NotificarDecision([FromBody] DecisionDTO datos)
        {
            try
            {
                _logger.LogInformation($"📧 Procesando decisión: {datos.Decision}");

                string nuevoEstado = datos.Decision == "aceptar" ? "Seleccionado" : "Rechazado";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string queryUpdate = @"
                        UPDATE Postulacion 
                        SET estado = @estado, fechaModificacion = GETDATE() 
                        WHERE idPostulacion = @idPostulacion";

                    using (SqlCommand cmd = new SqlCommand(queryUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                        cmd.Parameters.AddWithValue("@idPostulacion", datos.IdPostulacion);

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                            return NotFound(new { exito = false, mensaje = "Postulación no encontrada" });
                    }
                }

                // Enviar a webhook de n8n
                using (var httpClient = new HttpClient())
                {
                    var payload = new
                    {
                        idCandidato = datos.IdCandidato,
                        idPostulacion = datos.IdPostulacion,
                        decision = datos.Decision
                    };

                    var jsonContent = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(payload),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    var response = await httpClient.PostAsync(_n8nWebhookUrl, jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var resultContent = await response.Content.ReadAsStringAsync();
                        return Ok(new
                        {
                            exito = true,
                            mensaje = "Decisión procesada y notificación enviada",
                            respuestaN8n = resultContent
                        });
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, new
                        {
                            exito = false,
                            mensaje = "Error al comunicarse con n8n",
                            detalles = errorContent
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error al notificar decisión");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error interno al procesar notificación",
                    error = ex.Message
                });
            }
        }

        // OBTENER DATOS PARA NOTIFICACIÓN
        [HttpPost("obtener-datos-notificacion")]
        public IActionResult ObtenerDatosParaNotificacion([FromBody] DatosNotificacionRequestDTO datos)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            CAST(u.idUsuario AS INT) as idCandidato,
                            u.nombre, u.correo, u.whatsappNumber,
                            CAST(p.idPostulacion AS INT) as idPostulacion,
                            c.titulo as tituloConvocatoria
                        FROM Usuario u
                        INNER JOIN Candidato cand ON u.idUsuario = cand.idCandidato
                        INNER JOIN Postulacion p ON cand.idCandidato = p.idCandidato
                        INNER JOIN Convocatoria c ON p.idConvocatoria = c.idConvocatoria
                        WHERE u.idUsuario = @idCandidato 
                          AND p.idPostulacion = @idPostulacion";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idCandidato", datos.IdCandidato);
                        cmd.Parameters.AddWithValue("@idPostulacion", datos.IdPostulacion);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var resultado = new
                                {
                                    idCandidato = reader.GetInt32(0),
                                    nombre = reader.GetString(1),
                                    correo = reader.GetString(2),
                                    whatsappNumber = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    idPostulacion = reader.GetInt32(4),
                                    tituloConvocatoria = reader.GetString(5)
                                };

                                return Ok(resultado);
                            }

                            return NotFound(new
                            {
                                exito = false,
                                mensaje = "No se encontró el candidato o postulación"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos para notificación");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error inesperado al preparar notificación",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region 📦 DTOs

        public class ScoreDTO
        {
            public int Score { get; set; }
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
            public string TelegramId { get; set; }
            public int IdConvocatoria { get; set; }
            public int? IdCandidato { get; set; }
        }

        public class ActualizarPerfilDTO
        {
            public string Nombre { get; set; }
            public string Telefono { get; set; }
            public string NivelFormacion { get; set; }
            public string Experiencia { get; set; }
            public string Documento { get; set; }
            public DateTime? FechaNacimiento { get; set; }
        }

        public class ActualizarEstadoDTO
        {
            public string Estado { get; set; }
        }

        public class DecisionDTO
        {
            public int IdCandidato { get; set; }
            public int IdPostulacion { get; set; }
            public string Decision { get; set; }
        }

        public class DatosNotificacionRequestDTO
        {
            public int IdCandidato { get; set; }
            public int IdPostulacion { get; set; }
        }

        #endregion
    }
}
using BLL;
using ENTITY;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

        // 🔹 1. BUSCAR CONVOCATORIAS VENCIDAS (n8n ejecuta esto automáticamente)
        [HttpGet("vencidas")]
        public IActionResult ObtenerConvocatoriasVencidas()
        {
            _logger.LogInformation("🕐 Buscando convocatorias vencidas...");

            try
            {
                List<dynamic> convocatorias = new List<dynamic>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    idConvocatoria,
                    titulo,
                    descripcion,
                    fechaFin
                FROM Convocatoria
                WHERE fechaFin < GETDATE() 
                  AND estado = 'Activa'
                ORDER BY fechaFin DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
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
                }

                _logger.LogInformation("✓ Se encontraron {Count} convocatorias vencidas", convocatorias.Count);

                return Ok(new
                {
                    total = convocatorias.Count,
                    convocatorias = convocatorias
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al buscar convocatorias vencidas");
                return StatusCode(500, new { mensaje = "Error", error = ex.Message });
            }
        }

        // 🔹 3. CERRAR CONVOCATORIA (cuando n8n termina de evaluar)
        [HttpPut("{idConvocatoria}/cerrar")]
        public IActionResult CerrarConvocatoria(int idConvocatoria)
        {
            _logger.LogInformation("🔒 Cerrando convocatoria {Id}", idConvocatoria);

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                UPDATE Convocatoria
                SET estado = 'Cerrada'
                WHERE idConvocatoria = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idConvocatoria);

                        int filas = cmd.ExecuteNonQuery();

                        if (filas > 0)
                        {
                            _logger.LogInformation("✓ Convocatoria {Id} cerrada", idConvocatoria);
                            return Ok(new { exito = true, mensaje = "Convocatoria cerrada" });
                        }
                        else
                        {
                            return NotFound(new { exito = false, mensaje = "Convocatoria no encontrada" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al cerrar convocatoria");
                return StatusCode(500, new { mensaje = "Error", error = ex.Message });
            }
        }

        // 🔹 4. OBTENER TOP 3 CANDIDATOS (para el correo de notificación)
        [HttpGet("{idConvocatoria}/top-candidatos")]
        public IActionResult ObtenerTopCandidatos(int idConvocatoria)
        {
            _logger.LogInformation("🏆 Obteniendo top candidatos de convocatoria {Id}", idConvocatoria);

            try
            {
                List<dynamic> candidatos = new List<dynamic>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT TOP 3
                    u.nombre,
                    u.correo,
                    p.score
                FROM Postulacion p
                INNER JOIN Candidato c ON p.idCandidato = c.idCandidato
                INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                WHERE p.idConvocatoria = @id
                  AND p.score IS NOT NULL
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

                _logger.LogInformation("✓ Top {Count} candidatos obtenidos", candidatos.Count);

                return Ok(new { candidatos = candidatos });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener top candidatos");
                return StatusCode(500, new { mensaje = "Error", error = ex.Message });
            }
        }

        #region 📄 GESTIÓN DE CVs (NUEVO)

        /// <summary>
        /// Sube un CV en PDF y lo guarda como binario en la BD
        /// </summary>
        [HttpPost("{idCandidato}/subir-cv")]
        public async Task<IActionResult> SubirCV(int idCandidato, IFormFile archivoPdf)
        {
            _logger.LogInformation("📤 Subiendo CV para candidato: {Id}", idCandidato);

            try
            {
                // Validaciones
                if (archivoPdf == null || archivoPdf.Length == 0)
                    return BadRequest(new { exito = false, mensaje = "No se recibió ningún archivo" });

                if (!archivoPdf.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { exito = false, mensaje = "Solo se permiten archivos PDF" });

                if (archivoPdf.Length > 5 * 1024 * 1024) // 5 MB
                    return BadRequest(new { exito = false, mensaje = "El archivo excede 5 MB" });

                // Convertir PDF a byte[]
                byte[] pdfBytes;
                using (var ms = new MemoryStream())
                {
                    await archivoPdf.CopyToAsync(ms);
                    pdfBytes = ms.ToArray();
                }

                // Guardar en la base de datos
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Verificar que el candidato existe
                    string queryVerificar = "SELECT COUNT(*) FROM Candidato WHERE idCandidato = @id";
                    using (SqlCommand cmdVerificar = new SqlCommand(queryVerificar, conn))
                    {
                        cmdVerificar.Parameters.AddWithValue("@id", idCandidato);
                        int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());

                        if (existe == 0)
                            return NotFound(new { exito = false, mensaje = "Candidato no encontrado" });
                    }

                    // Actualizar el CV
                    string queryUpdate = @"
                        UPDATE Candidato 
                        SET hojaDeVida = @pdf 
                        WHERE idCandidato = @id";

                    using (SqlCommand cmd = new SqlCommand(queryUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idCandidato);
                        cmd.Parameters.Add("@pdf", System.Data.SqlDbType.VarBinary).Value = pdfBytes;

                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            _logger.LogInformation("✅ CV guardado correctamente para candidato {Id}", idCandidato);

                            // Extraer texto para mostrar preview
                            string textoExtraido = "";
                            try
                            {
                                textoExtraido = PdfService.ExtraerTextoDePdf(pdfBytes);
                            }
                            catch (Exception exPdf)
                            {
                                _logger.LogWarning("⚠️ No se pudo extraer texto del PDF: {Msg}", exPdf.Message);
                            }

                            return Ok(new
                            {
                                exito = true,
                                mensaje = "CV subido correctamente",
                                idCandidato = idCandidato,
                                tamañoArchivo = $"{archivoPdf.Length / 1024} KB",
                                caracteresExtraidos = textoExtraido.Length,
                                vistaPrevia = textoExtraido.Length > 0
                                    ? textoExtraido.Substring(0, Math.Min(300, textoExtraido.Length)) + "..."
                                    : "No se pudo extraer texto"
                            });
                        }

                        return StatusCode(500, new { exito = false, mensaje = "No se pudo actualizar el CV" });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al subir CV");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al subir CV",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Descarga el CV de un candidato como PDF
        /// </summary>
        [HttpGet("{idCandidato}/descargar-cv")]
        public IActionResult DescargarCV(int idCandidato)
        {
            _logger.LogInformation("📥 Descargando CV del candidato: {Id}", idCandidato);

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT u.nombre, c.hojaDeVida 
                        FROM Candidato c
                        INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                        WHERE c.idCandidato = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idCandidato);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.IsDBNull(1))
                                    return NotFound(new { exito = false, mensaje = "Este candidato no tiene CV" });

                                string nombre = reader.GetString(0);
                                byte[] pdfBytes = (byte[])reader["hojaDeVida"];

                                _logger.LogInformation("✅ CV descargado para {Nombre}", nombre);

                                return File(pdfBytes, "application/pdf", $"CV_{nombre}.pdf");
                            }

                            return NotFound(new { exito = false, mensaje = "Candidato no encontrado" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al descargar CV");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al descargar CV",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 🔥 ENDPOINT CRÍTICO: Este es el que usa n8n para obtener candidatos con CV en texto
        /// </summary>
        [HttpGet("convocatoria/{idConvocatoria}/candidatos")]
        public IActionResult ObtenerCandidatosConCV(int idConvocatoria)
        {
            _logger.LogInformation("🤖 n8n solicitando candidatos de convocatoria: {Id}", idConvocatoria);

            try
            {
                var candidatos = new List<object>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            p.idPostulacion,
                            u.nombre,
                            u.correo,
                            c.hojaDeVida,
                            p.score
                        FROM Postulacion p
                        INNER JOIN Candidato c ON p.idCandidato = c.idCandidato
                        INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                        WHERE p.idConvocatoria = @id
                        ORDER BY p.fechaPostulacion DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idConvocatoria);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int idPostulacion = reader.GetInt32(0);
                                string nombre = reader.GetString(1);
                                string correo = reader.GetString(2);
                                byte[] pdfBytes = reader.IsDBNull(3) ? null : (byte[])reader["hojaDeVida"];
                                decimal? score = reader.IsDBNull(4) ? null : (decimal?)reader.GetDecimal(4);

                                // 🔥 EXTRAER TEXTO DEL PDF
                                string textoCV = "Sin CV disponible";
                                if (pdfBytes != null && pdfBytes.Length > 0)
                                {
                                    try
                                    {
                                        textoCV = PdfService.ExtraerTextoDePdf(pdfBytes);
                                        _logger.LogInformation("✅ Texto extraído para {Nombre}: {Chars} caracteres",
                                            nombre, textoCV.Length);
                                    }
                                    catch (Exception exPdf)
                                    {
                                        _logger.LogWarning("⚠️ Error extrayendo PDF de {Nombre}: {Msg}",
                                            nombre, exPdf.Message);
                                        textoCV = "Error al extraer texto del CV";
                                    }
                                }

                                candidatos.Add(new
                                {
                                    idPostulacion = idPostulacion,
                                    nombre = nombre,
                                    correo = correo,
                                    hojaDeVida = textoCV, // ← Esto es lo que recibe n8n
                                    score = score
                                });
                            }
                        }
                    }
                }

                _logger.LogInformation("✅ Retornando {Count} candidatos a n8n", candidatos.Count);

                return Ok(new
                {
                    idConvocatoria = idConvocatoria,
                    candidatos = candidatos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener candidatos para n8n");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al obtener candidatos",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region 📝 REGISTRO Y POSTULACIONES (TU CÓDIGO ORIGINAL)

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

        /// <summary>
        /// 🤖 ACTUALIZAR SCORE (usado por n8n)
        /// </summary>
        [HttpPut("{id}/score")]
        public IActionResult ActualizarScore(int id, [FromBody] ScoreDTO datos)
        {
            _logger.LogInformation("🤖 n8n actualizando score de postulación: {Id}", id);

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
                            _logger.LogInformation("✅ Score actualizado: {Id} = {Score}", id, datos.Score);

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
                _logger.LogError(ex, "❌ Error al actualizar score");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al actualizar score",
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
        }

        #endregion
    }
}

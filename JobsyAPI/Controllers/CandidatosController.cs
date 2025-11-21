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

        #region 🔹 CONSULTAS DE CONVOCATORIAS

        // 1. OBTENER TODAS (Para candidatos)
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
                        SELECT 
                            c.idConvocatoria, c.titulo, c.descripcion, 
                            c.fechaPublicacion, c.fechaLimite, c.estado,
                            (SELECT COUNT(*) FROM Postulacion p WHERE p.idConvocatoria = c.idConvocatoria) as totalPostulaciones
                        FROM Convocatoria c
                        ORDER BY c.idConvocatoria DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
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
                return Ok(new { exito = true, total = convocatorias.Count, convocatorias = convocatorias });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error interno", error = ex.Message });
            }
        }

        // 2. 🟢 NUEVO: OBTENER SOLO LAS DEL RECLUTADOR
        [HttpGet("reclutador/{idReclutador}/convocatorias")]
        public IActionResult ObtenerConvocatoriasPorReclutador(int idReclutador)
        {
            try
            {
                List<dynamic> convocatorias = new List<dynamic>();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 🔍 Query filtrada por idReclutador
                    string query = @"
                        SELECT 
                            c.idConvocatoria, c.titulo, c.descripcion, 
                            c.fechaPublicacion, c.fechaLimite, c.estado,
                            (SELECT COUNT(*) FROM Postulacion p WHERE p.idConvocatoria = c.idConvocatoria) as totalPostulaciones
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
                return Ok(new { exito = true, total = convocatorias.Count, convocatorias = convocatorias });
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
                    string query = "SELECT idConvocatoria, titulo, descripcion, fechaLimite FROM Convocatoria WHERE fechaLimite < GETDATE() AND estado = 'Abierta' ORDER BY fechaLimite DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                convocatorias.Add(new { idConvocatoria = reader.GetInt32(0), titulo = reader.GetString(1), descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2), fechaLimite = reader.GetDateTime(3).ToString("yyyy-MM-dd") });
                            }
                        }
                    }
                }
                return Ok(new { total = convocatorias.Count, convocatorias = convocatorias });
            }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error", error = ex.Message }); }
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
                        if (filas > 0) return Ok(new { exito = true, mensaje = "Convocatoria cerrada" });
                        return NotFound(new { exito = false, mensaje = "Convocatoria no encontrada" });
                    }
                }
            }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error", error = ex.Message }); }
        }

        [HttpGet("{idConvocatoria}/top-candidatos")]
        public IActionResult ObtenerTopCandidatos(int idConvocatoria)
        {
            try
            {
                List<dynamic> candidatos = new List<dynamic>();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT TOP 3 u.nombre, u.correo, p.score FROM Postulacion p INNER JOIN Candidato c ON p.idCandidato = c.idCandidato INNER JOIN Usuario u ON c.idCandidato = u.idUsuario WHERE p.idConvocatoria = @id AND p.score IS NOT NULL ORDER BY p.score DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idConvocatoria);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                candidatos.Add(new { nombre = reader.GetString(0), correo = reader.GetString(1), score = Math.Round(reader.GetDecimal(2), 1) });
                            }
                        }
                    }
                }
                return Ok(new { candidatos = candidatos });
            }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error", error = ex.Message }); }
        }

        #endregion

        #region 📄 GESTIÓN DE CVs Y PERFIL

        [HttpPost("{idCandidato}/subir-cv")]
        public async Task<IActionResult> SubirCV(int idCandidato, IFormFile archivoPdf)
        {
            try
            {
                if (archivoPdf == null || archivoPdf.Length == 0) return BadRequest(new { exito = false, mensaje = "No archivo" });
                byte[] pdfBytes;
                using (var ms = new MemoryStream()) { await archivoPdf.CopyToAsync(ms); pdfBytes = ms.ToArray(); }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryUpdate = "UPDATE Candidato SET hojaDeVida = @pdf WHERE idCandidato = @id";
                    using (SqlCommand cmd = new SqlCommand(queryUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idCandidato);
                        cmd.Parameters.Add("@pdf", System.Data.SqlDbType.VarBinary).Value = pdfBytes;
                        int filas = cmd.ExecuteNonQuery();
                        if (filas > 0) return Ok(new { exito = true, mensaje = "CV subido" });
                        return StatusCode(500, new { exito = false, mensaje = "No se actualizó" });
                    }
                }
            }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error", error = ex.Message }); }
        }

        [HttpGet("{idCandidato}/descargar-cv")]
        public IActionResult DescargarCV(int idCandidato)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT u.nombre, c.hojaDeVida FROM Candidato c JOIN Usuario u ON c.idCandidato = u.idUsuario WHERE c.idCandidato = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idCandidato);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.IsDBNull(1)) return NotFound(new { exito = false, mensaje = "Sin CV" });
                                byte[] pdfBytes = (byte[])reader["hojaDeVida"];
                                return File(pdfBytes, "application/pdf", $"CV.pdf");
                            }
                            return NotFound(new { exito = false, mensaje = "Candidato no encontrado" });
                        }
                    }
                }
            }
            catch (Exception ex) { return StatusCode(500, new { exito = false, mensaje = "Error", error = ex.Message }); }
        }

        [HttpGet("convocatoria/{idConvocatoria}/candidatos")]
        public IActionResult ObtenerCandidatosConCV(int idConvocatoria)
        {
            try
            {
                var candidatos = new List<object>();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    // 🟢 CAMBIO: Agregamos p.idCandidato a la selección
                    string query = @"
                SELECT p.idPostulacion, p.idCandidato, u.nombre, u.correo, c.hojaDeVida, p.score, p.estado
                FROM Postulacion p
                INNER JOIN Candidato c ON p.idCandidato = c.idCandidato
                INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                WHERE p.idConvocatoria = @id
                ORDER BY p.score DESC"; // Ordenamos por mejor puntuación

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
                                    try { textoCV = PdfService.ExtraerTextoDePdf(pdfBytes); } catch { textoCV = "Error lectura PDF"; }
                                }

                                candidatos.Add(new
                                {
                                    idPostulacion = reader.GetInt32(0),
                                    idCandidato = reader.GetInt32(1), // 🟢 Ahora sí lo tenemos
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

        [HttpGet("estado/{identificador}")]
        public IActionResult ObtenerEstadoCandidato(string identificador)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryUsuario = "SELECT u.idUsuario, u.nombre, u.correo, c.nivelFormacion, c.experiencia, u.whatsappNumber, u.documento, u.fechaNacimiento, u.foto FROM Usuario u INNER JOIN Candidato c ON u.idUsuario = c.idCandidato WHERE u.telegramId = @id OR CAST(u.idUsuario AS VARCHAR) = @id";
                    using (SqlCommand cmd = new SqlCommand(queryUsuario, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", identificador);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read()) return NotFound(new { encontrado = false });
                            byte[] fotoBytes = reader.IsDBNull(8) ? null : (byte[])reader["foto"];
                            string fotoBase64 = fotoBytes != null ? "data:image/jpeg;base64," + Convert.ToBase64String(fotoBytes) : null;

                            var candidato = new
                            {
                                id = reader.GetInt32(0),
                                nombre = reader.GetString(1),
                                correo = reader.GetString(2),
                                nivelFormacion = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                experiencia = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                documento = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                foto = fotoBase64
                            };
                            reader.Close();

                            // Postulaciones
                            List<dynamic> postulaciones = new List<dynamic>();
                            string queryPost = "SELECT p.idPostulacion, p.fechaPostulacion, p.estado, c.titulo FROM Postulacion p INNER JOIN Convocatoria c ON p.idConvocatoria = c.idConvocatoria WHERE p.idCandidato = @idUsuario ORDER BY p.fechaPostulacion DESC";
                            using (SqlCommand cmdPost = new SqlCommand(queryPost, conn))
                            {
                                cmdPost.Parameters.AddWithValue("@idUsuario", candidato.id);
                                using (SqlDataReader r2 = cmdPost.ExecuteReader())
                                {
                                    while (r2.Read()) postulaciones.Add(new { id = r2.GetInt32(0), fecha = r2.GetDateTime(1).ToString("yyyy-MM-dd"), estado = r2.GetString(2), convocatoria = r2.GetString(3) });
                                }
                            }
                            return Ok(new { encontrado = true, candidato, postulaciones, totalPostulaciones = postulaciones.Count });
                        }
                    }
                }
            }
            catch (Exception ex) { return StatusCode(500, new { exito = false, mensaje = "Error", error = ex.Message }); }
        }

        [HttpPut("perfil/{id}")]
        public IActionResult ActualizarPerfil(int id, [FromBody] ActualizarPerfilDTO dto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string qUser = "UPDATE Usuario SET nombre = @nombre, whatsappNumber = @tel, documento = @doc, fechaNacimiento = @fecha WHERE idUsuario = @id";
                    using (SqlCommand cmd = new SqlCommand(qUser, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", dto.Nombre);
                        cmd.Parameters.AddWithValue("@tel", dto.Telefono ?? "");
                        cmd.Parameters.AddWithValue("@doc", dto.Documento ?? "");
                        cmd.Parameters.AddWithValue("@fecha", (object)dto.FechaNacimiento ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    string qCand = "UPDATE Candidato SET nivelFormacion = @form, experiencia = @exp WHERE idCandidato = @id";
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
            catch (Exception ex) { return StatusCode(500, new { exito = false, mensaje = "Error", error = ex.Message }); }
        }

        #endregion

        #region 📝 REGISTRO Y POSTULACION

        [HttpPost("registro")]
        public IActionResult RegistrarCandidato([FromBody] RegistroCandidatoDTO dto)
        {
            // (Misma lógica de registro rápido para bot)
            return Ok(new { exito = true, mensaje = "Registrado" });
        }

        [HttpPost("postular")]
        public IActionResult PostularAConvocatoria([FromBody] PostulacionDTO dto)
        {
            try
            {
                if (dto == null || dto.IdConvocatoria <= 0) return BadRequest(new { exito = false, mensaje = "Datos inválidos" });
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    int idUsuario = 0;
                    if (dto.IdCandidato.HasValue && dto.IdCandidato.Value > 0) idUsuario = dto.IdCandidato.Value;
                    else if (!string.IsNullOrWhiteSpace(dto.TelegramId))
                    {
                        string q = "SELECT idUsuario FROM Usuario WHERE telegramId = @tid";
                        using (SqlCommand cmd = new SqlCommand(q, conn)) { cmd.Parameters.AddWithValue("@tid", dto.TelegramId); var r = cmd.ExecuteScalar(); if (r==null) return NotFound(new { exito = false }); idUsuario = Convert.ToInt32(r); }
                    }
                    else return BadRequest(new { exito = false, mensaje = "Falta ID" });

                    string qCheck = "SELECT COUNT(*) FROM Postulacion WHERE idCandidato=@uid AND idConvocatoria=@cid";
                    using (SqlCommand cmd = new SqlCommand(qCheck, conn)) { cmd.Parameters.AddWithValue("@uid", idUsuario); cmd.Parameters.AddWithValue("@cid", dto.IdConvocatoria); if (Convert.ToInt32(cmd.ExecuteScalar()) > 0) return Ok(new { exito = false, mensaje = "Ya postulado" }); }

                    string qIns = "INSERT INTO Postulacion (idCandidato, idConvocatoria, fechaPostulacion, estado) VALUES (@uid, @cid, GETDATE(), 'Pendiente'); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand cmd = new SqlCommand(qIns, conn)) { cmd.Parameters.AddWithValue("@uid", idUsuario); cmd.Parameters.AddWithValue("@cid", dto.IdConvocatoria); int id = Convert.ToInt32(cmd.ExecuteScalar()); return Ok(new { exito = true, idPostulacion = id, mensaje = "Postulación exitosa" }); }
                }
            }
            catch (Exception ex) { return StatusCode(500, new { exito = false, mensaje = "Error", error = ex.Message }); }
        }

        [HttpPut("{id}/score")]
        public IActionResult ActualizarScore(int id, [FromBody] ScoreDTO datos) { return Ok(new { exito = true }); }

        #endregion

        #region 📦 DTOs
        public class ScoreDTO { public int Score { get; set; } }
        public class RegistroCandidatoDTO { public string Nombre { get; set; } public string Correo { get; set; } public string TelegramId { get; set; } public string TelegramUsername { get; set; } public string WhatsappNumber { get; set; } }
        public class PostulacionDTO { public string TelegramId { get; set; } public int IdConvocatoria { get; set; } public int? IdCandidato { get; set; } }
        public class ActualizarPerfilDTO { public string Nombre { get; set; } public string Telefono { get; set; } public string NivelFormacion { get; set; } public string Experiencia { get; set; } public string Documento { get; set; } public DateTime? FechaNacimiento { get; set; } }
        #endregion
    }
}
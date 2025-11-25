using BLL; // Necesario para PdfService
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace JobsyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConvocatoriasController : ControllerBase
    {
        private readonly string _connectionString;

        public ConvocatoriasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("JobsyDB");
        }

        // 🔹 1. OBTENER CONVOCATORIAS VENCIDAS (Para n8n)
        // 🔹 1. OBTENER CONVOCATORIAS VENCIDAS (Para n8n) - MODIFICADO
        [HttpGet("vencidas")]
        public IActionResult ObtenerConvocatoriasVencidas()
        {
            try
            {
                var convocatorias = new List<object>();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 🟢 QUERY MODIFICADO: Ahora incluye datos del reclutador
                    string query = @"
                SELECT 
                    c.idConvocatoria, 
                    c.titulo, 
                    c.descripcion, 
                    c.fechaLimite,
                    u.correo as correoReclutador,
                    u.nombre as nombreReclutador,
                    c.idReclutador
                FROM Convocatoria c
                INNER JOIN Usuario u ON c.idReclutador = u.idUsuario
                WHERE c.fechaLimite <= CAST(GETDATE() AS DATE)
                  AND c.estado = 'Abierta'";

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
                                    fechaLimite = reader.GetDateTime(3).ToString("yyyy-MM-dd"),
                                    correoReclutador = reader.GetString(4),      // 🟢 NUEVO
                                    nombreReclutador = reader.GetString(5),      // 🟢 NUEVO
                                    idReclutador = reader.GetInt32(6)            // 🟢 NUEVO
                                });
                            }
                        }
                    }
                }
                return Ok(new { exito = true, total = convocatorias.Count, convocatorias = convocatorias });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error", error = ex.Message });
            }
        }

        // 🔹 0. CREAR CONVOCATORIA (USADO POR EL FRONTEND)
        [HttpPost("crear")]
        public IActionResult CrearConvocatoria([FromBody] JsonElement data)
        {
            try
            {
                string titulo = data.GetProperty("Titulo").GetString();
                string descripcion = data.GetProperty("Descripcion").GetString();
                DateTime fechaPublicacion = DateTime.Parse(data.GetProperty("FechaPublicacion").GetString());
                DateTime fechaLimite = DateTime.Parse(data.GetProperty("FechaLimite").GetString());
                string estado = data.GetProperty("Estado").GetString();
                int idEmpresa = data.GetProperty("IdEmpresa").GetInt32();
                int idReclutador = data.GetProperty("IdReclutador").GetInt32();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                INSERT INTO Convocatoria (titulo, descripcion, fechaPublicacion, fechaLimite, estado, idEmpresa, idReclutador)
                OUTPUT INSERTED.idConvocatoria
                VALUES (@titulo, @descripcion, @fechaPublicacion, @fechaLimite, @estado, @idEmpresa, @idReclutador)
            ";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@titulo", titulo);
                        cmd.Parameters.AddWithValue("@descripcion", descripcion);
                        cmd.Parameters.AddWithValue("@fechaPublicacion", fechaPublicacion);
                        cmd.Parameters.AddWithValue("@fechaLimite", fechaLimite);
                        cmd.Parameters.AddWithValue("@estado", estado);
                        cmd.Parameters.AddWithValue("@idEmpresa", idEmpresa);
                        cmd.Parameters.AddWithValue("@idReclutador", idReclutador);

                        int idInsertado = (int)cmd.ExecuteScalar();

                        return Ok(new
                        {
                            exito = true,
                            mensaje = "Convocatoria creada correctamente",
                            idConvocatoria = idInsertado
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al crear convocatoria",
                    error = ex.Message
                });
            }
        }


        // 🔹 2. OBTENER CANDIDATOS DE UNA CONVOCATORIA (CORREGIDO)
        // URL: GET /api/convocatorias/5/candidatos
        [HttpGet("{id}/candidatos")]
        public IActionResult ObtenerCandidatos(int id)
        {
            try
            {
                var candidatos = new List<object>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 🚨 CORRECCIÓN: Agregamos p.score y estandarizamos nombres
                    string query = @"
                        SELECT 
                            p.idPostulacion,
                            p.idCandidato,
                            p.estado,
                            c.hojaDeVida,
                            c.experiencia,
                            u.nombre,   -- Índice 5
                            u.correo,   -- Índice 6
                            p.score     -- Índice 7 (NUEVO)
                        FROM Postulacion p
                        INNER JOIN Candidato c ON p.idCandidato = c.idCandidato
                        INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                        WHERE p.idConvocatoria = @idConvocatoria
                        ORDER BY p.score DESC"; // Ordenar por mejor puntaje

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idConvocatoria", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Extraer texto del PDF
                                string hojaDeVidaTexto = "Sin CV disponible";
                                if (!reader.IsDBNull(3))
                                {
                                    byte[] pdfBytes = (byte[])reader.GetValue(3);
                                    if (pdfBytes.Length > 0)
                                    {
                                        try { hojaDeVidaTexto = PdfService.ExtraerTextoDePdf(pdfBytes); }
                                        catch { hojaDeVidaTexto = "Error lectura PDF"; }
                                    }
                                }

                                // 🟢 MAPEADO CORREGIDO PARA EL FRONTEND
                                candidatos.Add(new
                                {
                                    idPostulacion = reader.GetInt32(0),
                                    idCandidato = reader.GetInt32(1),
                                    estado = reader.GetString(2),
                                    hojaDeVida = hojaDeVidaTexto,
                                    experiencia = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    nombre = reader.GetString(5), // Antes era 'nombreCandidato', ahora 'nombre'
                                    correo = reader.GetString(6), // Antes era 'correoCandidato', ahora 'correo'
                                    score = reader.IsDBNull(7) ? 0 : Convert.ToInt32(reader.GetValue(7)) // 🟢 Leemos el Score
                                });
                            }
                        }
                    }
                }

                return Ok(new
                {
                    exito = true,
                    total = candidatos.Count,
                    candidatos = candidatos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error al obtener candidatos", error = ex.Message });
            }
        }

        // 🔹 3. CERRAR CONVOCATORIA
        [HttpPut("{id}/Cerrar")]
        public IActionResult CerrarConvocatoria(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Convocatoria SET estado = 'Cerrada' WHERE idConvocatoria = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int filas = cmd.ExecuteNonQuery();
                        if (filas > 0) return Ok(new { exito = true, mensaje = "Convocatoria cerrada correctamente" });
                        return NotFound(new { exito = false, mensaje = "Convocatoria no encontrada" });
                    }
                }
            }
            catch (Exception ex) { return StatusCode(500, new { exito = false, mensaje = "Error", error = ex.Message }); }
        }

        // 🔹 4. TOP CANDIDATOS (Para n8n)
        [HttpGet("{id}/top-candidatos")]
        public IActionResult ObtenerTopCandidatos(int id)
        {
            try
            {
                var topCandidatos = new List<object>();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT TOP 3 u.nombre, p.score, u.correo
                        FROM Postulacion p
                        INNER JOIN Candidato c ON p.idCandidato = c.idCandidato
                        INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                        WHERE p.idConvocatoria = @id AND p.score IS NOT NULL
                        ORDER BY p.score DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                topCandidatos.Add(new
                                {
                                    nombre = reader.GetString(0),
                                    score = reader.GetValue(1),
                                    correo = reader.GetString(2)
                                });
                            }
                        }
                    }
                }
                return Ok(new { exito = true, total = topCandidatos.Count, candidatos = topCandidatos });
            }
            catch (Exception ex) { return StatusCode(500, new { exito = false, mensaje = "Error", error = ex.Message }); }
        }
    }
}
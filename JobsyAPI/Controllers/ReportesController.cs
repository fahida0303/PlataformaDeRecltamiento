using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace JobsyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly string _connectionString;

        public ReportesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("JobsyDB");
        }

        // ========================================
        // 📊 ENDPOINT 1: Métricas semanales
        // ========================================
        [HttpGet("metricas-semanales")]
        public IActionResult ObtenerMetricasSemanales()
        {
            // (Este lo dejamos igual o lo actualizas luego si también quieres filtrar gráficos históricos)
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            COUNT(*) AS totalPostulaciones,
                            SUM(CASE WHEN estado = 'Seleccionado' THEN 1 ELSE 0 END) AS seleccionados,
                            SUM(CASE WHEN estado = 'Rechazado' THEN 1 ELSE 0 END) AS rechazados,
                            SUM(CASE WHEN estado = 'Pendiente' THEN 1 ELSE 0 END) AS pendientes
                        FROM Postulacion
                        WHERE fechaPostulacion >= DATEADD(DAY, -7, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Ok(new
                                {
                                    exito = true,
                                    periodo = "Últimos 7 días",
                                    metricas = new
                                    {
                                        totalPostulaciones = reader.GetInt32(0),
                                        seleccionados = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                        rechazados = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                                        pendientes = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                                    }
                                });
                            }
                        }
                    }
                }
                return Ok(new { exito = false, mensaje = "No hay datos" });
            }
            catch (Exception ex) { return StatusCode(500, new { exito = false, mensaje = ex.Message }); }
        }

        // ========================================
        // 📅 ENDPOINT 2: Resumen diario (FILTRADO POR RECLUTADOR)
        // URL: GET /api/reportes/resumen-diario?idReclutador=5
        // ========================================
        [HttpGet("resumen-diario")]
        public IActionResult ObtenerResumenDiario([FromQuery] int? idReclutador = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Lógica dinámica para el filtro
                    // Si hay idReclutador, filtramos por las convocatorias de ESE reclutador
                    string filtroReclutador = idReclutador.HasValue ? " AND c.idReclutador = @idReclutador " : "";
                    string filtroConvocatoria = idReclutador.HasValue ? " AND idReclutador = @idReclutador " : "";

                    string query = $@"
                        SELECT 
                            -- 1. Postulaciones de hoy (filtradas si es necesario)
                            (SELECT COUNT(*) 
                             FROM Postulacion p
                             INNER JOIN Convocatoria c ON p.idConvocatoria = c.idConvocatoria
                             WHERE CAST(p.fechaPostulacion AS DATE) = CAST(GETDATE() AS DATE)
                             {filtroReclutador}) AS postulacionesHoy,

                            -- 2. Pendientes totales (filtradas)
                            (SELECT COUNT(*) 
                             FROM Postulacion p
                             INNER JOIN Convocatoria c ON p.idConvocatoria = c.idConvocatoria
                             WHERE p.estado = 'Pendiente'
                             {filtroReclutador}) AS totalPendientes,

                            -- 3. Convocatorias abiertas (filtradas)
                            (SELECT COUNT(*) 
                             FROM Convocatoria 
                             WHERE estado = 'Abierta' 
                             {filtroConvocatoria}) AS convocatoriasAbiertas";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (idReclutador.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@idReclutador", idReclutador.Value);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int postulacionesHoy = reader.GetInt32(0);
                                int totalPendientes = reader.GetInt32(1);
                                int convocatoriasAbiertas = reader.GetInt32(2);

                                return Ok(new
                                {
                                    exito = true,
                                    fecha = DateTime.Now.ToString("yyyy-MM-dd"),
                                    resumen = new
                                    {
                                        postulacionesHoy = postulacionesHoy,
                                        totalPendientes = totalPendientes,
                                        convocatoriasAbiertas = convocatoriasAbiertas
                                    }
                                });
                            }
                        }
                    }
                }
                return Ok(new { exito = false, mensaje = "No hay datos" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, mensaje = "Error al generar resumen", error = ex.Message });
            }
        }

        // ========================================
        // 🏆 ENDPOINT 3: Ranking (Sin cambios)
        // ========================================
        [HttpGet("ranking/{idConvocatoria}")]
        public IActionResult ObtenerRanking(int idConvocatoria)
        {
            // ... (Mismo código que ya tenías) ...
            // Por brevedad no lo repito, pero mantenlo igual.
            try
            {
                var ranking = new List<object>();
                string tituloConvocatoria = "";
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string queryTitulo = "SELECT titulo FROM Convocatoria WHERE idConvocatoria = @id";
                    using (SqlCommand cmd = new SqlCommand(queryTitulo, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idConvocatoria);
                        var result = cmd.ExecuteScalar();
                        if (result == null) return NotFound(new { exito = false, mensaje = "Convocatoria no encontrada" });
                        tituloConvocatoria = result.ToString();
                    }
                    string queryRanking = @"SELECT TOP 10 u.nombre, p.score, p.estado FROM Postulacion p INNER JOIN Usuario u ON p.idCandidato = u.idUsuario WHERE p.idConvocatoria = @id AND p.score IS NOT NULL ORDER BY p.score DESC";
                    using (SqlCommand cmd = new SqlCommand(queryRanking, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idConvocatoria);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int posicion = 1;
                            while (reader.Read())
                            {
                                ranking.Add(new { posicion = posicion++, nombre = reader.GetString(0), score = reader.GetInt32(1), estado = reader.GetString(2) });
                            }
                        }
                    }
                }
                return Ok(new { exito = true, convocatoria = tituloConvocatoria, totalCandidatos = ranking.Count, rranking = ranking });
            }
            catch (Exception ex) { return StatusCode(500, new { exito = false, mensaje = ex.Message }); }
        }
    }
}
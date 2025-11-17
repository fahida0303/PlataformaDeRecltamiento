using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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

        
        // URL: GET /api/convocatorias/vencidas
        [HttpGet("vencidas")]
        public IActionResult ObtenerConvocatoriasVencidas()
        {
            try
            {
                
                var convocatorias = new List<object>();

              
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    
                    string query = @"
                        SELECT 
                            idConvocatoria,
                            titulo,
                            descripcion,
                            fechaLimite
                        FROM Convocatoria
                        WHERE fechaLimite <= CAST(GETDATE() AS DATE)
                          AND estado = 'Abierta'";

                    
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

                
                return Ok(new
                {
                    exito = true,
                    total = convocatorias.Count,
                    convocatorias = convocatorias
                });
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al obtener convocatorias",
                    error = ex.Message
                });
            }
        }

     
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

                    string query = @"
                        SELECT 
                            p.idPostulacion,
                            p.idCandidato,
                            p.estado,
                            c.hojaDeVida,
                            c.experiencia,
                            u.nombre,
                            u.correo
                        FROM Postulacion p
                        INNER JOIN Candidato c ON p.idCandidato = c.idCandidato
                        INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                        WHERE p.idConvocatoria = @idConvocatoria
                          AND p.estado = 'Aplicado'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@idConvocatoria", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                candidatos.Add(new
                                {
                                    idPostulacion = reader.GetInt32(0),
                                    idCandidato = reader.GetInt32(1),
                                    estado = reader.GetString(2),
                                    hojaDeVida = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    experiencia = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    nombreCandidato = reader.GetString(5),
                                    correoCandidato = reader.GetString(6)
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
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al obtener candidatos",
                    error = ex.Message
                });
            }
        }

  
        // URL: PUT /api/convocatorias/5/cerrar
        [HttpPut("{id}/Cerrada")]
        public IActionResult CerrarConvocatoria(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
                        UPDATE Convocatoria 
                        SET estado = 'Cerrada'
                        WHERE idConvocatoria = @idConvocatoria";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idConvocatoria", id);


                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            return Ok(new
                            {
                                exito = true,
                                mensaje = "Convocatoria cerrada correctamente"
                            });
                        }
                        else
                        {
                            return NotFound(new
                            {
                                exito = false,
                                mensaje = "Convocatoria no encontrada"
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
                    mensaje = "Error al cerrar convocatoria",
                    error = ex.Message
                });
            }
        }


        // URL: GET /api/convocatorias/5/top-candidatos
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
                        SELECT TOP 3
                            u.nombre,
                            p.score,
                            u.correo
                        FROM Postulacion p
                        INNER JOIN Candidato c ON p.idCandidato = c.idCandidato
                        INNER JOIN Usuario u ON c.idCandidato = u.idUsuario
                        WHERE p.idConvocatoria = @idConvocatoria
                          AND p.estado = 'Evaluado'
                          AND p.score IS NOT NULL
                        ORDER BY p.score DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idConvocatoria", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                topCandidatos.Add(new
                                {
                                    nombre = reader.GetString(0),
                                    score = reader.GetInt32(1),
                                    correo = reader.GetString(2)
                                });
                            }
                        }
                    }
                }

                return Ok(new
                {
                    exito = true,
                    total = topCandidatos.Count,
                    candidatos = topCandidatos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error al obtener top candidatos",
                    error = ex.Message
                });
            }
        }
    }
}

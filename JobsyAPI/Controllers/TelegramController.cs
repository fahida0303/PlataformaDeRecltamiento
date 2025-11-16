using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace JobsyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TelegramController> _logger;
        private readonly HttpClient _httpClient;

        public TelegramController(IConfiguration configuration, ILogger<TelegramController> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// 📤 Enviar mensaje de Telegram
        /// URL: POST /api/telegram/send
        /// Body: { "telegramId": "123456789", "mensaje": "Hola mundo" }
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> EnviarMensaje([FromBody] TelegramMensajeDTO datos)
        {
            _logger.LogInformation("📤 Enviando mensaje Telegram a: {TelegramId}", datos?.TelegramId);

            try
            {
                // Validación
                if (datos == null || string.IsNullOrWhiteSpace(datos.TelegramId) || string.IsNullOrWhiteSpace(datos.Mensaje))
                {
                    return BadRequest(new
                    {
                        exito = false,
                        mensaje = "TelegramId y Mensaje son obligatorios"
                    });
                }

                // Obtener token de configuración (appsettings.json)
                string botToken = _configuration["Telegram:BotToken"];

                if (string.IsNullOrWhiteSpace(botToken))
                {
                    _logger.LogError("❌ Token de bot de Telegram no configurado");
                    return StatusCode(500, new
                    {
                        exito = false,
                        mensaje = "Token de bot no configurado"
                    });
                }

                // Construir URL de la API de Telegram
                string telegramApiUrl = $"https://api.telegram.org/bot{botToken}/sendMessage";

                // Preparar el payload
                var payload = new
                {
                    chat_id = datos.TelegramId,
                    text = datos.Mensaje,
                    parse_mode = "Markdown" // Para formatear el mensaje
                };

                string jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Enviar a Telegram
                HttpResponseMessage response = await _httpClient.PostAsync(telegramApiUrl, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("✓ Mensaje enviado exitosamente a {TelegramId}", datos.TelegramId);

                    return Ok(new
                    {
                        exito = true,
                        mensaje = "Mensaje enviado correctamente",
                        telegramId = datos.TelegramId
                    });
                }
                else
                {
                    _logger.LogWarning("⚠️ Error al enviar mensaje: {Response}", responseBody);

                    return StatusCode((int)response.StatusCode, new
                    {
                        exito = false,
                        mensaje = "Error al enviar mensaje de Telegram",
                        detalle = responseBody
                    });
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "❌ Error de conexión con Telegram API");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error de conexión con Telegram",
                    error = httpEx.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error inesperado al enviar mensaje");
                return StatusCode(500, new
                {
                    exito = false,
                    mensaje = "Error inesperado",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 🏥 Health check
        /// URL: GET /api/telegram/health
        /// </summary>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            string botToken = _configuration["Telegram:BotToken"];
            bool tokenConfigurado = !string.IsNullOrWhiteSpace(botToken);

            return Ok(new
            {
                status = "healthy",
                servicio = "TelegramController",
                tokenConfigurado = tokenConfigurado,
                timestamp = DateTime.Now
            });
        }
    }

    /// <summary>
    /// DTO para recibir datos de mensaje de Telegram
    /// </summary>
    public class TelegramMensajeDTO
    {
        public string TelegramId { get; set; }
        public string Mensaje { get; set; }
    }
}

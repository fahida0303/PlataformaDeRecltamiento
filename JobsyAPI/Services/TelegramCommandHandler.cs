using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace JobsyAPI.Services
{
    public class TelegramCommandHandler
    {
        private readonly string _connectionString;
        private readonly ILogger<TelegramCommandHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _n8nWebhookUrl;

        public TelegramCommandHandler(
            IConfiguration configuration,
            ILogger<TelegramCommandHandler> logger,
            IHttpClientFactory httpClientFactory)
        {
            _connectionString = configuration.GetConnectionString("JobsyDB");
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _n8nWebhookUrl = configuration["N8n:WebhookUrl"] ?? "";
        }

        // ✅ CAMBIO: Retorna tupla con InlineKeyboardMarkup (tipo concreto)
        public async Task<(string message, InlineKeyboardMarkup keyboard)> HandleCommand(
            string command,
            long chatId,
            Message message)
        {
            var telegramId = chatId.ToString();

            try
            {
                return command.ToLower() switch
                {
                    "/start" => await HandleStart(telegramId, message),
                    "/convocatorias" => await HandleConvocatorias(telegramId),
                    "/mispostulaciones" => await HandleMisPostulaciones(telegramId),
                    "/perfil" => await HandlePerfil(telegramId),
                    "/ayuda" or "/help" => await HandleAyuda(telegramId),
                    "/misconvocatorias" => await HandleMisConvocatorias(telegramId),
                    "/ranking" => await HandleRanking(telegramId, command),
                    "/reporte" => await HandleReporte(telegramId),
                    _ => ("❓ Comando no reconocido. Usa /ayuda", null)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en comando: {Command}", command);
                return ("❌ Error procesando comando. Intenta de nuevo.", null);
            }
        }

        public async Task<string> HandleCallback(string callbackData, long chatId)
        {
            try
            {
                if (callbackData.StartsWith("postular_"))
                {
                    var idConvo = int.Parse(callbackData.Replace("postular_", ""));
                    return await HandlePostular(chatId.ToString(), idConvo);
                }

                return "❌ Acción no reconocida";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en callback: {Data}", callbackData);
                return "❌ Error procesando acción";
            }
        }

        private async Task<(string, InlineKeyboardMarkup)> HandleStart(string telegramId, Message message)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmdCheck = new SqlCommand(
                    "SELECT nombre FROM Usuario WHERE telegramId = @tid", conn);
                cmdCheck.Parameters.AddWithValue("@tid", telegramId);

                var nombre = await cmdCheck.ExecuteScalarAsync();
                if (nombre != null)
                {
                    return ($"👋 ¡Hola {nombre}!\n\nUsa /ayuda para ver comandos.", null);
                }

                var firstName = message.From.FirstName ?? "Usuario";
                var username = message.From.Username ?? "";
                var correo = $"{(string.IsNullOrEmpty(username) ? telegramId : username)}@telegram.temp";

                var cmdUser = new SqlCommand(
                    @"INSERT INTO Usuario (nombre, correo, contraseña, estado, telegramId, telegramUsername, fechaUltimaInteraccionBot)
                      VALUES (@nombre, @correo, @pass, 'Activo', @tid, @tusername, GETDATE());
                      SELECT SCOPE_IDENTITY();", conn);

                cmdUser.Parameters.AddWithValue("@nombre", firstName);
                cmdUser.Parameters.AddWithValue("@correo", correo);
                cmdUser.Parameters.AddWithValue("@pass", $"tg_{DateTime.Now.Ticks}");
                cmdUser.Parameters.AddWithValue("@tid", telegramId);
                cmdUser.Parameters.AddWithValue("@tusername", username);

                var idUsuario = Convert.ToInt32(await cmdUser.ExecuteScalarAsync());

                var cmdCand = new SqlCommand(
                    "INSERT INTO Candidato (idCandidato, tipox) VALUES (@id, 'Externo')", conn);
                cmdCand.Parameters.AddWithValue("@id", idUsuario);
                await cmdCand.ExecuteNonQueryAsync();

                var mensajeBienvenida = $"🎉 ¡Bienvenido {firstName}!\n\n" +
                       "✅ Registro exitoso\n\n" +
                       "📋 /convocatorias - Ver ofertas\n" +
                       "📊 /mispostulaciones - Tus postulaciones\n" +
                       "👤 /perfil - Tu perfil\n" +
                       "❓ /ayuda - Ver comandos";

                return (mensajeBienvenida, null);
            }
        }

        private async Task<(string, InlineKeyboardMarkup)> HandleConvocatorias(string telegramId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmd = new SqlCommand(
                    @"SELECT TOP 10 idConvocatoria, titulo, descripcion, fechaLimite 
                      FROM Convocatoria 
                      WHERE estado = 'Abierta' AND fechaLimite > GETDATE()
                      ORDER BY fechaPublicacion DESC", conn);

                var convos = new List<(int id, string titulo, string desc, DateTime fecha)>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        convos.Add((
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetDateTime(3)
                        ));
                    }
                }

                if (convos.Count == 0)
                    return ("📭 No hay convocatorias abiertas.", null);

                var mensaje = new StringBuilder("📢 *CONVOCATORIAS ABIERTAS*\n\n");
                var buttons = new List<InlineKeyboardButton[]>();

                foreach (var (id, titulo, desc, fecha) in convos)
                {
                    var descCorta = desc.Length > 100 ? desc.Substring(0, 97) + "..." : desc;

                    var tituloLimpio = EscapeMarkdown(titulo);
                    var descLimpia = EscapeMarkdown(descCorta);

                    mensaje.AppendLine($"📋 *{tituloLimpio}*");
                    mensaje.AppendLine($"{descLimpia}");
                    mensaje.AppendLine($"📅 Vence: {fecha:dd/MM/yyyy}");
                    mensaje.AppendLine();

                    buttons.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData($"✅ Postular a: {titulo}", $"postular_{id}")
                    });
                }

                var keyboard = new InlineKeyboardMarkup(buttons);
                return (mensaje.ToString(), keyboard);
            }
        }

        private async Task<(string, InlineKeyboardMarkup)> HandleMisPostulaciones(string telegramId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmdUser = new SqlCommand(
                    "SELECT idUsuario FROM Usuario WHERE telegramId = @tid", conn);
                cmdUser.Parameters.AddWithValue("@tid", telegramId);

                var idUsuario = await cmdUser.ExecuteScalarAsync();
                if (idUsuario == null)
                    return ("❌ Usa /start primero", null);

                var cmd = new SqlCommand(
                    @"SELECT c.titulo, p.fecha_postulacion, p.estado, ISNULL(p.score, 0)
                      FROM Postulacion p
                      INNER JOIN Convocatoria c ON p.idConvocatoria = c.idConvocatoria
                      WHERE p.idCandidato = @id
                      ORDER BY p.fecha_postulacion DESC", conn);
                cmd.Parameters.AddWithValue("@id", idUsuario);

                var posts = new List<string>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var titulo = reader.GetString(0);
                        var fecha = reader.GetDateTime(1);
                        var estado = reader.GetString(2);
                        var score = reader.IsDBNull(3) ? 0 : Convert.ToInt32(reader.GetDecimal(3));

                        var emoji = estado switch
                        {
                            "Seleccionado" => "✅",
                            "Rechazado" => "❌",
                            "Evaluado" => "📊",
                            _ => "⏳"
                        };

                        posts.Add(
                            $"{emoji} *{EscapeMarkdown(titulo)}*\n" +
                            $"   Estado: {estado}\n" +
                            $"   {(score > 0 ? $"Score: {score}%\n   " : "")}" +
                            $"Fecha: {fecha:dd/MM}"
                        );
                    }
                }

                if (posts.Count == 0)
                    return ("📭 No tienes postulaciones.\n\nUsa /convocatorias", null);

                return ("📊 *MIS POSTULACIONES*\n\n" + string.Join("\n\n", posts), null);
            }
        }

        private async Task<(string, InlineKeyboardMarkup)> HandlePerfil(string telegramId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmd = new SqlCommand(
                    @"SELECT u.nombre, u.correo, c.nivelFormacion, 
                             COUNT(p.idPostulacion),
                             u.tipoUsuario
                      FROM Usuario u
                      INNER JOIN Candidato c ON u.idUsuario = c.idCandidato
                      LEFT JOIN Postulacion p ON c.idCandidato = p.idCandidato
                      WHERE u.telegramId = @tid
                      GROUP BY u.nombre, u.correo, c.nivelFormacion, u.tipoUsuario", conn);
                cmd.Parameters.AddWithValue("@tid", telegramId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var nombre = reader.GetString(0);
                        var correo = reader.GetString(1);
                        var nivel = reader.IsDBNull(2) ? "No especificado" : reader.GetString(2);
                        var totalPost = reader.GetInt32(3);
                        var tipoUsuario = reader.IsDBNull(4) ? "Candidato" : reader.GetString(4);

                        var rolEmoji = tipoUsuario == "Reclutador" ? "👔" : "👤";

                        return ($"{rolEmoji} *TU PERFIL*\n\n" +
                               $"📛 {nombre}\n" +
                               $"📧 {correo}\n" +
                               $"🎓 {nivel}\n" +
                               $"🏷️ Rol: {tipoUsuario}\n" +
                               $"📊 Postulaciones: {totalPost}", null);
                    }
                }

                return ("❌ Perfil no encontrado. Usa /start", null);
            }
        }

        private async Task<string> HandlePostular(string telegramId, int idConvo)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmdUser = new SqlCommand(
                    "SELECT idUsuario FROM Usuario WHERE telegramId = @tid", conn);
                cmdUser.Parameters.AddWithValue("@tid", telegramId);

                var idUsuario = await cmdUser.ExecuteScalarAsync();
                if (idUsuario == null)
                    return "❌ Usa /start primero";

                var cmdCheck = new SqlCommand(
                    "SELECT COUNT(*) FROM Postulacion WHERE idCandidato = @user AND idConvocatoria = @convo", conn);
                cmdCheck.Parameters.AddWithValue("@user", idUsuario);
                cmdCheck.Parameters.AddWithValue("@convo", idConvo);

                if (Convert.ToInt32(await cmdCheck.ExecuteScalarAsync()) > 0)
                    return "⚠️ Ya te postulaste a esta convocatoria";

                var cmdPost = new SqlCommand(
                    @"INSERT INTO Postulacion (idCandidato, idConvocatoria, fecha_postulacion, estado)
                      VALUES (@user, @convo, GETDATE(), 'Pendiente')", conn);
                cmdPost.Parameters.AddWithValue("@user", idUsuario);
                cmdPost.Parameters.AddWithValue("@convo", idConvo);

                await cmdPost.ExecuteNonQueryAsync();

                return "🎉 *¡Postulación exitosa!*\n\n" +
                       "📊 Estado: Pendiente\n" +
                       "📧 Te notificaremos los resultados\n\n" +
                       "Usa /mispostulaciones para ver el estado";
            }
        }

        private async Task<(string, InlineKeyboardMarkup)> HandleMisConvocatorias(string telegramId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmdUser = new SqlCommand(
                    "SELECT idUsuario, tipoUsuario FROM Usuario WHERE telegramId = @tid", conn);
                cmdUser.Parameters.AddWithValue("@tid", telegramId);

                using (var reader = await cmdUser.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return ("❌ Usa /start primero", null);

                    var tipoUsuario = reader.IsDBNull(1) ? "Candidato" : reader.GetString(1);
                    if (tipoUsuario != "Reclutador")
                        return ("❌ Este comando es solo para reclutadores", null);
                }

                var cmd = new SqlCommand(
                    @"SELECT c.idConvocatoria, c.titulo, c.estado, 
                             COUNT(p.idPostulacion) as totalPostulaciones
                      FROM Convocatoria c
                      LEFT JOIN Postulacion p ON c.idConvocatoria = p.idConvocatoria
                      WHERE c.idReclutador = (SELECT idUsuario FROM Usuario WHERE telegramId = @tid)
                      GROUP BY c.idConvocatoria, c.titulo, c.estado
                      ORDER BY c.fechaPublicacion DESC", conn);
                cmd.Parameters.AddWithValue("@tid", telegramId);

                var convos = new List<string>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var id = reader.GetInt32(0);
                        var titulo = reader.GetString(1);
                        var estado = reader.GetString(2);
                        var total = reader.GetInt32(3);

                        var emoji = estado == "Abierta" ? "🟢" : "🔴";

                        convos.Add(
                            $"{emoji} *{EscapeMarkdown(titulo)}*\n" +
                            $"   Estado: {estado}\n" +
                            $"   Postulaciones: {total}\n" +
                            $"   Ver ranking: /ranking {id}"
                        );
                    }
                }

                if (convos.Count == 0)
                    return ("📭 No tienes convocatorias creadas", null);

                return ("📊 *MIS CONVOCATORIAS*\n\n" + string.Join("\n\n", convos), null);
            }
        }

        private async Task<(string, InlineKeyboardMarkup)> HandleRanking(string telegramId, string command)
        {
            return ("🔧 Función en desarrollo. Próximamente disponible.", null);
        }

        private async Task<(string, InlineKeyboardMarkup)> HandleReporte(string telegramId)
        {
            return ("🔧 Función en desarrollo. Próximamente disponible.", null);
        }

        private async Task<(string, InlineKeyboardMarkup)> HandleAyuda(string telegramId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var cmd = new SqlCommand(
                    "SELECT tipoUsuario FROM Usuario WHERE telegramId = @tid", conn);
                cmd.Parameters.AddWithValue("@tid", telegramId);

                var tipoUsuario = (await cmd.ExecuteScalarAsync())?.ToString() ?? "Candidato";

                if (tipoUsuario == "Reclutador")
                {
                    return ("🤖 *COMANDOS - RECLUTADOR*\n\n" +
                           "📋 /misconvocatorias - Tus convocatorias\n" +
                           "📊 /ranking [ID] - Top candidatos\n" +
                           "📈 /reporte - Reporte diario\n" +
                           "👤 /perfil - Tu información\n" +
                           "❓ /ayuda - Este mensaje", null);
                }
                else
                {
                    return ("🤖 *COMANDOS - CANDIDATO*\n\n" +
                           "📋 /convocatorias - Ver ofertas\n" +
                           "📊 /mispostulaciones - Tus postulaciones\n" +
                           "👤 /perfil - Tu información\n" +
                           "❓ /ayuda - Este mensaje\n\n" +
                           "💡 Usa los botones para postularte", null);
                }
            }
        }

        private string EscapeMarkdown(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            return text
                .Replace("_", "\\_")
                .Replace("*", "\\*")
                .Replace("[", "\\[")
                .Replace("]", "\\]")
                .Replace("`", "\\`");
        }
    }
}
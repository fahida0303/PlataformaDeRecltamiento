using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace JobsyAPI.Services
{
    public class TelegramBotService : BackgroundService
    {
        private readonly ILogger<TelegramBotService> _logger;
        private readonly IConfiguration _configuration;
        private readonly TelegramCommandHandler _commandHandler;
        private TelegramBotClient _botClient;

        public TelegramBotService(
            ILogger<TelegramBotService> logger,
            IConfiguration configuration,
            TelegramCommandHandler commandHandler)
        {
            _logger = logger;
            _configuration = configuration;
            _commandHandler = commandHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var token = _configuration["TelegramBot:Token"] ?? _configuration["Telegram:BotToken"];

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogError("❌ Token de Telegram no configurado");
                return;
            }

            try
            {
                _botClient = new TelegramBotClient(token);

                var me = await _botClient.GetMe(stoppingToken);
                _logger.LogInformation("✅ Bot iniciado: @{Username} (ID: {Id})", me.Username, me.Id);

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = new[]
                    {
                        UpdateType.Message,
                        UpdateType.CallbackQuery
                    }
                };

                _botClient.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    stoppingToken
                );

                _logger.LogInformation("🤖 Bot ACTIVO y escuchando mensajes");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error crítico al iniciar bot: {Message}", ex.Message);
            }
        }

        private async Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken)
        {
            try
            {
                // Manejar mensajes de texto (comandos)
                if (update.Message is { } message && message.Text is { } messageText)
                {
                    await HandleTextMessage(botClient, message, messageText, cancellationToken);
                }
                // Manejar clicks en botones inline
                else if (update.CallbackQuery is { } callbackQuery)
                {
                    await HandleCallbackQuery(botClient, callbackQuery, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error procesando update");
            }
        }

        private async Task HandleTextMessage(
            ITelegramBotClient botClient,
            Message message,
            string messageText,
            CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            var username = message.From?.Username ?? "Usuario";

            _logger.LogInformation("📩 @{Username} ({ChatId}): {Text}", username, chatId, messageText);

            try
            {
                var (respuesta, keyboard) = await _commandHandler.HandleCommand(
                    messageText,
                    chatId,
                    message
                );

                await botClient.SendMessage(
                    chatId,
                    respuesta,
                    parseMode: ParseMode.Markdown,
                    replyMarkup: keyboard,
                    cancellationToken: cancellationToken
                );

                _logger.LogInformation("✅ Respuesta enviada a @{Username}", username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error procesando mensaje de @{Username}", username);

                try
                {
                    await botClient.SendMessage(
                        chatId,
                        "❌ Error procesando tu mensaje. Intenta de nuevo.",
                        cancellationToken: cancellationToken
                    );
                }
                catch { }
            }
        }

        private async Task HandleCallbackQuery(
            ITelegramBotClient botClient,
            CallbackQuery callbackQuery,
            CancellationToken cancellationToken)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var username = callbackQuery.From.Username ?? "Usuario";
            var data = callbackQuery.Data;

            _logger.LogInformation("🔘 @{Username} presionó botón: {Data}", username, data);

            try
            {
                // Responder inmediatamente para quitar el "loading" del botón
                await botClient.AnswerCallbackQuery(
                    callbackQuery.Id,
                    cancellationToken: cancellationToken
                );

                var respuesta = await _commandHandler.HandleCallback(data, chatId);

                await botClient.SendMessage(
                    chatId,
                    respuesta,
                    parseMode: ParseMode.Markdown,
                    cancellationToken: cancellationToken
                );

                _logger.LogInformation("✅ Callback procesado para @{Username}", username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error procesando callback de @{Username}", username);

                try
                {
                    await botClient.SendMessage(
                        chatId,
                        "❌ Error procesando tu acción. Intenta de nuevo.",
                        cancellationToken: cancellationToken
                    );
                }
                catch { }
            }
        }

        private Task HandleErrorAsync(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiEx => $"Telegram API Error [{apiEx.ErrorCode}]: {apiEx.Message}",
                _ => exception.Message
            };

            _logger.LogError("❌ Error en Polling: {Error}", errorMessage);
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("⚠️ Deteniendo bot de Telegram...");
            await base.StopAsync(cancellationToken);
            _logger.LogInformation("🛑 Bot detenido");
        }
    }
}
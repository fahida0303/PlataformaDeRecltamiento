using JobsyAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// SERVICIOS
// =====================================================

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Jobsy API",
        Version = "v1",
        Description = "API para el sistema de reclutamiento Jobsy"
    });
});

// HttpClient (para llamadas externas)
builder.Services.AddHttpClient();

// =====================================================
// CORS (React + n8n + Telegram)
// =====================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Servicios de Telegram
builder.Services.AddSingleton<TelegramCommandHandler>();
builder.Services.AddHostedService<TelegramBotService>();

// Logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

// =====================================================
// BUILD
// =====================================================
var app = builder.Build();

// =====================================================
// MIDDLEWARE PIPELINE
// =====================================================

// Swagger (solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Jobsy API v1");
        c.RoutePrefix = "swagger";
    });
}

// 🚨 CORS debe ir aquí — ANTES de authorization, controllers, telegram, TODO.
app.UseCors("AllowAll");

// Redirección HTTPS (desactivado por desarrollo)
// app.UseHttpsRedirection();

// Authorization
app.UseAuthorization();

// Controllers
app.MapControllers();

// =====================================================
// HEALTH CHECK
// =====================================================
app.MapGet("/health", () => new
{
    status = "healthy",
    timestamp = DateTime.Now,
    service = "Jobsy API",
    version = "1.0.0"
});

// =====================================================
// LOGS DE ARRANQUE
// =====================================================
app.Lifetime.ApplicationStarted.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("╔════════════════════════════════════════╗");
    logger.LogInformation("║       JOBSY API - INICIADO             ║");
    logger.LogInformation("╠════════════════════════════════════════╣");
    logger.LogInformation("║  📊 Swagger: http://localhost:5026     ║");
    logger.LogInformation("║  🤖 Bot Telegram: ACTIVO               ║");
    logger.LogInformation("║  🔗 n8n Webhooks: LISTOS               ║");
    logger.LogInformation("╚════════════════════════════════════════╝");
});

// =====================================================
// RUN
// =====================================================
app.Run();

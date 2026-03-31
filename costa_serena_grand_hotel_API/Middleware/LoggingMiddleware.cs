using System.Security.Claims;
using System.Text;
using System.Text.Json;
using costa_serena_grand_hotel_API.Data;
using costa_serena_grand_hotel_API.Models;

namespace costa_serena_grand_hotel_API.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, HotelDbContext dbContext)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (ShouldSkip(path))
            {
                await _next(context);
                return;
            }

            var method = context.Request.Method;
            var timestamp = DateTime.UtcNow;

            var userId =
                context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                context.User?.FindFirst("sub")?.Value;

            var userEmail =
                context.User?.FindFirst(ClaimTypes.Email)?.Value ??
                context.User?.FindFirst("email")?.Value;

            string? attemptedEmail = null;

            if (string.IsNullOrWhiteSpace(userEmail) &&
                HttpMethods.IsPost(method) &&
                path.Equals("/api/auth/login", StringComparison.OrdinalIgnoreCase))
            {
                attemptedEmail = await TryReadLoginEmailAsync(context);
            }

            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            var (entityType, entityId, action) = ParsePathInfo(method, path);

            await _next(context);

            var effectiveEmail = userEmail ?? attemptedEmail;

            var statusCode = context.Response.StatusCode;
            var (logLevel, message, isAuthFailure) =
                DetermineLogDetails(method, path, statusCode, effectiveEmail);

            _logger.Log(
                GetLogLevel(logLevel),
                "{Method} {Path} - {StatusCode} - User: {UserEmail} - IP: {IpAddress}",
                method,
                path,
                statusCode,
                effectiveEmail ?? "Anonymous",
                ipAddress ?? "-"
            );

            var logEntry = new Log
            {
                Timestamp = timestamp,
                UserId = userId,
                UserEmail = effectiveEmail,
                HttpMethod = method,
                Path = path,
                StatusCode = statusCode,
                Message = message,
                LogLevel = logLevel,
                IsAuthFailure = isAuthFailure,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                EntityType = entityType,
                EntityId = entityId,
                Action = action
            };

            dbContext.Logs.Add(logEntry);
            await dbContext.SaveChangesAsync();
        }

        private static async Task<string?> TryReadLoginEmailAsync(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();

                context.Request.Body.Position = 0;

                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);

                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (string.IsNullOrWhiteSpace(body))
                    return null;

                using var json = JsonDocument.Parse(body);

                if (json.RootElement.TryGetProperty("email", out var emailProp))
                    return emailProp.GetString();

                if (json.RootElement.TryGetProperty("Email", out var emailProp2))
                    return emailProp2.GetString();

                return null;
            }
            catch
            {
                if (context.Request.Body.CanSeek)
                    context.Request.Body.Position = 0;

                return null;
            }
        }

        private static bool ShouldSkip(string path)
        {
            return path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)
                   || path.StartsWith("/favicon", StringComparison.OrdinalIgnoreCase)
                   || path.StartsWith("/_framework", StringComparison.OrdinalIgnoreCase);
        }

        private static (string logLevel, string message, bool isAuthFailure) DetermineLogDetails(
            string method,
            string path,
            int statusCode,
            string? userEmail)
        {
            var user = string.IsNullOrWhiteSpace(userEmail) ? "Anonymous" : userEmail;

            return statusCode switch
            {
                401 => ("Warning", $"Unauthorized access attempt by {user} to {method} {path}", true),
                403 => ("Warning", $"Forbidden access attempt by {user} to {method} {path}", true),
                >= 400 and < 500 => ("Warning", $"{user} - {method} {path} failed with {statusCode}", false),
                >= 500 => ("Error", $"Server error: {method} {path} - {statusCode}", false),
                _ => ("Information", $"{user} - {method} {path} - {statusCode}", false)
            };
        }

        private static (string? entityType, string? entityId, string? action) ParsePathInfo(
            string method,
            string path)
        {
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            string? entityType = null;
            string? entityId = null;
            string? action = null;

            if (parts.Length >= 2 && parts[0].Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                entityType = parts[1] switch
                {
                    "Auth" => "Auth",
                    "Vendeg" => "Vendeg",
                    "Szoba" => "Szoba",
                    "SzobaKategoria" => "SzobaKategoria",
                    "Foglalas" => "Foglalas",
                    "Ertekelesek" => "Ertekeles",
                    "Termek" => "Termek",
                    "Rendeles" => "Rendeles",
                    "Admin" => "Admin",
                    _ => parts[1]
                };

                if (parts.Length >= 3)
                {
                    if (int.TryParse(parts[2], out _) || parts[2].Length <= 50)
                        entityId = parts[2];
                    else
                        action = parts[2];
                }

                action ??= method switch
                {
                    "GET" => entityId != null ? "View" : "List",
                    "POST" => entityType == "Auth" ? "Login/Register" : "Create",
                    "PUT" => "Update",
                    "DELETE" => "Delete",
                    _ => method
                };
            }

            return (entityType, entityId, action);
        }

        private static LogLevel GetLogLevel(string level) => level switch
        {
            "Error" => LogLevel.Error,
            "Warning" => LogLevel.Warning,
            _ => LogLevel.Information
        };
    }
}
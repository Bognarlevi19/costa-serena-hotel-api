using System.Text.Json;
using costa_serena_grand_hotel_API.Data;
using costa_serena_grand_hotel_API.Models;

namespace costa_serena_grand_hotel_API.Services
{
    public class AuditLogger : IAuditLogger
    {
        private readonly HotelDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogger(HotelDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogChangeAsync<T>(
            string userId,
            string userEmail,
            string entityType,
            string entityId,
            string action,
            T? oldValue,
            T? newValue)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();
            var path = httpContext?.Request.Path.Value ?? string.Empty;
            var method = httpContext?.Request.Method ?? string.Empty;

            var log = new Log
            {
                Timestamp = DateTime.UtcNow,
                UserId = userId,
                UserEmail = userEmail,
                HttpMethod = method,
                Path = path,
                StatusCode = 200,
                Message = $"{action} művelet történt ezen: {entityType} #{entityId}",
                LogLevel = "Information",
                IsAuthFailure = false,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                OldValue = oldValue != null ? JsonSerializer.Serialize(oldValue) : null,
                NewValue = newValue != null ? JsonSerializer.Serialize(newValue) : null
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
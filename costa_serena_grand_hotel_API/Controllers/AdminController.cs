using costa_serena_grand_hotel_API.AdminModels;
using costa_serena_grand_hotel_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace costa_serena_grand_hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly HotelDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, HotelDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserActivityDto>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserActivityDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var lastLog = await _context.Logs
                    .Where(l => l.UserId == user.Id)
                    .OrderByDescending(l => l.Timestamp)
                    .FirstOrDefaultAsync();

                var totalActions = await _context.Logs.CountAsync(l => l.UserId == user.Id);
                var failedLogins = await _context.Logs.CountAsync(l => l.UserId == user.Id && l.IsAuthFailure);

                result.Add(new UserActivityDto
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Roles = roles.ToList(),
                    LastActivity = lastLog?.Timestamp,
                    LastActivityDescription = lastLog != null
                        ? $"{lastLog.HttpMethod} {lastLog.Path}"
                        : "Még nincs aktivitás",
                    TotalActions = totalActions,
                    FailedLoginAttempts = failedLogins
                });
            }

            return Ok(result.OrderByDescending(x => x.LastActivity));
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserDetailsDto>> GetUserDetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "A felhasználó nem található." });

            var roles = await _userManager.GetRolesAsync(user);

            var recentLogs = await _context.Logs
                .Where(l => l.UserId == id)
                .OrderByDescending(l => l.Timestamp)
                .Take(100)
                .Select(l => new LogDto
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    UserId = l.UserId,
                    UserEmail = l.UserEmail,
                    HttpMethod = l.HttpMethod,
                    Path = l.Path,
                    StatusCode = l.StatusCode,
                    Message = l.Message,
                    LogLevel = l.LogLevel,
                    IsAuthFailure = l.IsAuthFailure,
                    IpAddress = l.IpAddress,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    Action = l.Action
                })
                .ToListAsync();

            var totalActions = await _context.Logs.CountAsync(l => l.UserId == id);
            var failedLogins = await _context.Logs.CountAsync(l => l.UserId == id && l.IsAuthFailure);

            var result = new UserDetailsDto
            {
                User = new UserInfo
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Roles = roles.ToList()
                },
                Stats = new UserStats
                {
                    TotalActions = totalActions,
                    FailedLogins = failedLogins,
                    LastActivity = recentLogs.FirstOrDefault()?.Timestamp
                },
                RecentLogs = recentLogs
            };

            return Ok(result);
        }

        [HttpGet("logs")]
        public async Task<ActionResult<LogsPagedDto>> GetLogs(
            [FromQuery] string? userEmail = null,
            [FromQuery] string? entityType = null,
            [FromQuery] bool? isAuthFailure = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 200) pageSize = 200;

            var query = _context.Logs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                query = query.Where(l => l.UserEmail != null && l.UserEmail.Contains(userEmail));
            }

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                query = query.Where(l => l.EntityType == entityType);
            }

            if (isAuthFailure.HasValue)
            {
                query = query.Where(l => l.IsAuthFailure == isAuthFailure.Value);
            }

            var totalCount = await query.CountAsync();

            var logs = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LogDto
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    UserId = l.UserId,
                    UserEmail = l.UserEmail,
                    HttpMethod = l.HttpMethod,
                    Path = l.Path,
                    StatusCode = l.StatusCode,
                    Message = l.Message,
                    LogLevel = l.LogLevel,
                    IsAuthFailure = l.IsAuthFailure,
                    IpAddress = l.IpAddress,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    Action = l.Action
                })
                .ToListAsync();

            return Ok(new LogsPagedDto
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Logs = logs
            });
        }

        [HttpGet("stats")]
        public async Task<ActionResult<AdminStatsDto>> GetStats()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var today = DateTime.UtcNow.Date;

            var activeUsersToday = await _context.Logs
                .Where(l => l.Timestamp >= today && l.UserId != null)
                .Select(l => l.UserId)
                .Distinct()
                .CountAsync();

            var totalLogs = await _context.Logs.CountAsync();

            var failedLoginsToday = await _context.Logs
                .CountAsync(l => l.IsAuthFailure && l.Timestamp >= today);

            var topActions = await _context.Logs
                .Where(l => l.EntityType != null && l.Action != null)
                .GroupBy(l => new { l.EntityType, l.Action })
                .Select(g => new EntityActionCount
                {
                    EntityType = g.Key.EntityType ?? "",
                    Action = g.Key.Action ?? "",
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            return Ok(new AdminStatsDto
            {
                TotalUsers = totalUsers,
                ActiveUsersToday = activeUsersToday,
                TotalLogs = totalLogs,
                FailedLoginAttemptsToday = failedLoginsToday,
                TopActions = topActions
            });
        }

        [HttpGet("logs/failed-logins")]
        public async Task<ActionResult<IEnumerable<LogDto>>> GetFailedLogins([FromQuery] int days = 7)
        {
            var since = DateTime.UtcNow.AddDays(-days);

            var logs = await _context.Logs
                .Where(l => l.IsAuthFailure && l.Timestamp >= since)
                .OrderByDescending(l => l.Timestamp)
                .Select(l => new LogDto
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    UserId = l.UserId,
                    UserEmail = l.UserEmail,
                    HttpMethod = l.HttpMethod,
                    Path = l.Path,
                    StatusCode = l.StatusCode,
                    Message = l.Message,
                    LogLevel = l.LogLevel,
                    IsAuthFailure = l.IsAuthFailure,
                    IpAddress = l.IpAddress,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    Action = l.Action
                })
                .ToListAsync();

            return Ok(logs);
        }

        [HttpGet("logs/by-ip")]
        public async Task<ActionResult<IEnumerable<IpStatsDto>>> GetIpStats([FromQuery] int days = 7)
        {
            var since = DateTime.UtcNow.AddDays(-days);

            var stats = await _context.Logs
                .Where(l => l.IpAddress != null && l.Timestamp >= since)
                .GroupBy(l => l.IpAddress)
                .Select(g => new IpStatsDto
                {
                    IpAddress = g.Key ?? "",
                    TotalRequests = g.Count(),
                    FailedLogins = g.Count(x => x.IsAuthFailure),
                    UniqueUsers = g.Where(x => x.UserEmail != null).Select(x => x.UserEmail!).Distinct().Count(),
                    LastActivity = g.Max(x => x.Timestamp)
                })
                .OrderByDescending(x => x.TotalRequests)
                .ToListAsync();

            return Ok(stats);
        }
    }
}
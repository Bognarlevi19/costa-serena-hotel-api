namespace costa_serena_grand_hotel_API.AdminModels
{
    public class UserDetailsDto
    {
        public UserInfo User { get; set; } = new();
        public UserStats Stats { get; set; } = new();
        public List<LogDto> RecentLogs { get; set; } = new();
    }

    public class UserInfo
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }

    public class UserStats
    {
        public int TotalActions { get; set; }
        public int FailedLogins { get; set; }
        public DateTime? LastActivity { get; set; }
    }
}
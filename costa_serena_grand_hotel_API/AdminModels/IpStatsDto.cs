namespace costa_serena_grand_hotel_API.AdminModels
{
    public class IpStatsDto
    {
        public string IpAddress { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public int FailedLogins { get; set; }
        public int UniqueUsers { get; set; }
        public DateTime? LastActivity { get; set; }
    }
}
namespace costa_serena_grand_hotel_API.AdminModels
{
    public class LogsPagedDto
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<LogDto> Logs { get; set; } = new();
    }
}
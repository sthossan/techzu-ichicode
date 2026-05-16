namespace Server.Shared.Models
{
    public class ApiErrorDetails
    {
        public string Type { get; set; } = "https://tools.ietf.org/html/rfc9110#section-15.6.1";
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string Detail { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
    }
}

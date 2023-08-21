namespace DTO.DTOs.Responses
{
    public class ServicePointsResponse
    {
        public bool? error { get; set; }
        public string? service_price { get; set; }
        public string? error_description { get; set; }
        public bool? update_token { get; set; }
    }
}

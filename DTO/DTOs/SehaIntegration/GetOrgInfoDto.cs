using Newtonsoft.Json;

namespace DTO.DTOs.SehaIntegration
{
    /// <summary>
    /// Organization Info DTO
    /// </summary>
    public class GetOrgInfoDto
    {
        /// <summary>
        /// Organization Info DTO
        /// </summary>
        public GetOrgInfoDto(bool success, string message, OrganizationCategoryResponse data, string errorCode)
        {
            Success = success;
            Message = message;
            Data = data;
            ErrorCode = errorCode;
        }
        /// <summary>
        /// Success
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Message 
        /// </summary>
        [JsonProperty("msg")]
        public string Message { get; set; }
        /// <summary>
        /// Organization Category Response 
        /// </summary>
        public OrganizationCategoryResponse Data { get; set; }
        /// <summary>
        /// ErrorCode
        /// </summary>
        public string ErrorCode { get; set; }
    }
}


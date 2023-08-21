using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.SehaIntegration
{
    /// <summary>
    /// User Info DTO
    /// </summary>
    public class GetUserInfoDto
    {
        /// <summary>
        /// User Info DTO
        /// </summary>
        public GetUserInfoDto(bool success, string message, GetUserInfoDataDto data, string errorCode)
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
        /// Get User Info Data Dto 
        /// </summary>
        public GetUserInfoDataDto Data { get; set; }
        /// <summary>
        /// ErrorCode 
        /// </summary>
        public string ErrorCode { get; set; }
    }
}

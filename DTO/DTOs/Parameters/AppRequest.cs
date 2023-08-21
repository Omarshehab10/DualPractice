using System;

namespace DTO.DTOs.Parameters
{
    /// <summary>
    /// AppRequest
    /// </summary>
    public class AppRequest
    {
        /// <summary>
        /// Seha Access Token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// OrganizationId
        /// </summary>
        public int OrganizationId { get; set; }
        /// <summary>
        /// UserId
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// ServiceId
        /// </summary>
        public int ServiceId { get; set; } = 65;
        /// <summary>
        /// CurrentDate
        /// </summary>
        public DateTime CurrentDate
        {
            get
            {
                return DateTime.Now;
            }
        }
    }

    /// <summary>
    /// AppRequest of type T
    /// </summary>
    public class AppRequest<T> : AppRequest
    {
        /// <summary>
        /// Data object
        /// </summary>
        public T Data { get; set; }
    }
}

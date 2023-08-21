using System.ComponentModel.DataAnnotations;

namespace DTO.DTOs.Parameters
{
    /// <summary>
    /// Get Request Info
    /// </summary>
    public class GetRequestInfo
    {
        [Required]
        /// <summary>
        /// Request Service Code
        /// </summary>
        public string RequestServiceCode { get; set; }

        [Required]
        /// <summary>
        /// Approval
        /// </summary>
        public bool Approval { get; set; }
        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOs.Parameters
{
    /// <summary>
    /// Get Request Info V2
    /// </summary>
    public class GetRequestInfoV2
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
        /// <summary>
        /// Rejection Reason Id
        /// </summary>
        public int? RejectionReasonId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.Responses
{
    /// <summary>
    /// Get License To HLS Response
    /// </summary>
    public class GetLicenseToHLSResponse
    {
        /// <summary>
        /// License Number
        /// </summary>
        public string LicenseNumber { get; set; }
        /// <summary>
        /// License Status
        /// </summary>
        public int LicenseStatus { get; set; }
        /// <summary>
        /// License EndDate
        /// </summary>
        public DateTime? LicenseEndDate { get; set; }
        /// <summary>
        /// Establisment Name
        /// </summary>
        public string EstablismentName { get; set; }
        /// <summary>
        /// Establishment License Number
        /// </summary>
        public string EstablishmentLicenseNumber { get; set; }

    }
}

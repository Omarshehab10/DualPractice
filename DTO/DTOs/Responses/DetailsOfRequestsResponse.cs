using Common.Enums;
using DTO.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.Parameters
{
    /// <summary>
    /// Details Of Requests Request
    /// </summary>
    public class DetailsOfRequestsResponse : DoctorRequestsDto
    {
        /// <summary>
        /// Private GOV License Expiry Date
        /// </summary>
        public DateTime? PrivateEstablishmentLicenseExpiryDate { get; set; }
        /// <summary>
        /// Directorate
        /// </summary>
        public string Directorate { get; set; }
        /// <summary>
        /// Practitioner Full Name
        /// </summary>
        public string PractitionerFullName { get; set; }
        /// <summary>
        /// Approval Start Date
        /// </summary>
        public DateTime? ApprovalStartDate { get; set; }
        /// <summary>
        /// Approval End Date
        /// </summary>
        public DateTime? ApprovalEndDate { get; set; }
        /// <summary>
        /// Practitioner License Number
        /// </summary>
        public string PractitionerLicenseNumber { get; set; }
        /// <summary>
        /// Scfhs Registration Expiry Date
        /// </summary>
        public DateTime? ScfhsRegistrationExpiryDate { get; set; }
        /// <summary>
        /// Refusal Side
        /// </summary>
        public string RefusalSide { get; set; }
        /// <summary>
        /// Rejection Date
        /// </summary>
        public DateTime? RejectionDate { get; set; }
        /// <summary>
        /// Rrejection Reason
        /// </summary>
        public string RrejectionReason { get; set; }
        /// <summary>
        /// List Of Day Schedule Info
        /// </summary>
    }
}

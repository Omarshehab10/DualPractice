using Common.Enums;
using DTO.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTO.DTOs.Parameters
{
    /// <summary>
    /// Add New Dual Practitioner Request
    /// </summary>
    public class AddNewDualPractitionerRequest
    {
        /// <summary>
        /// Practitioner Info
        /// </summary>
        public PractitionerInfo PractitionerInfo { get; set; }
        /// <summary>
        /// Private Establishment Info
        /// </summary>
        public PrivateEstablishmentInfo PrivateEstablishmentInfo { get; set; }
        /// <summary>
        /// Gov Establishment Info
        /// </summary>
        public GovEstablishmentInfo GovEstablishmentInfo { get; set; }
        /// <summary>
        /// DUAL Practitioner Request Info
        /// </summary>
        public DPRequestInfo DPRequestInfo { get; set; }
        /// <summary>
        /// List Of Day Schedule Info
        /// </summary>
        public List<DayScheduleInfo> DayScheduleInfo { get; set; }

    }
    /// <summary>
    /// Practitioner Info
    /// </summary>
    public class PractitionerInfo
    {
        /// <summary>
        /// National Id
        /// </summary>
        [Required]
        public string NationalId { get; set; }
        /// <summary>
        /// Scfhs Registration Number
        /// </summary>
        public string ScfhsRegistrationNumber { get; set; }
        /// <summary>
        /// Scfhs Registration Expiry Date
        /// </summary>
        public DateTime? ScfhsRegistrationExpiryDate { get; set; }
        /// <summary>
        /// Date Of BirthH
        /// </summary>
        public string DateOfBirthH { get; set; }
        /// <summary>
        /// Date Of BirthG
        /// </summary>
        public DateTime DateOfBirthG { get; set; }
        /// <summary>
        /// Gender
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// Full Name Arabic
        /// </summary>
        public string FullNameAr { get; set; }
        /// <summary>
        /// Full Name English
        /// </summary>
        public string FullNameEn { get; set; }
        /// <summary>
        /// License Number
        /// </summary>
        public string LicenseNumber { get; set; }
        /// <summary>
        /// Specialty Code
        /// </summary> 
        public string SpecialtyCode { get; set; }
        /// <summary>
        /// Specialty Name Ar
        /// </summary>
        public string SpecialtyNameAr { get; set; }
        /// <summary>
        /// Specialty Name English
        /// </summary>
        public string SpecialtyNameEn { get; set; }
        /// <summary>
        /// License Expiry Date
        /// </summary>
        public DateTime? LicenseExpiryDate { get; set; }
        /// <summary>
        /// Scfhs Category Ar
        /// </summary>
        public string ScfhsCategoryAr { get; set; }
        /// <summary>
        /// Scfhs Category En
        /// </summary>
        public string ScfhsCategoryEn { get; set; }

    }

    /// <summary>
    /// Private Establishment Info
    /// </summary>
    public class PrivateEstablishmentInfo
    {
        /// <summary>
        /// User Id
        /// </summary>
        [Required]
        public int UserId { get; set; }
        /// <summary>
        /// Organization Id
        /// </summary>
        [Required]
        public int OrganizationId { get; set; }
        /// <summary>
        /// License Expiry Date
        /// </summary>
        public DateTime? LicenseExpiryDate { get; set; }
    }
    /// <summary>
    /// Gov Establishment Info
    /// </summary>
    public class GovEstablishmentInfo
    {
        /// <summary>
        /// Organization Id
        /// </summary>
        [Required]
        public int OrganizationId { get; set; }
        /// <summary>
        /// Approval Level
        /// </summary>
        [Required]
        public int ApprovalLevel { get; set; }
        /// <summary>
        /// Name Ar
        /// </summary>
        public string NameAr { get; set; }
        /// <summary>
        /// Name En
        /// </summary>
        public string NameEn { get; set; }
        /// <summary>
        /// Cluster Id
        /// </summary>
        public string ClusterId { get; set; }
        /// <summary>
        /// ManagementArea Id
        /// </summary>
        public int ManagementAreaId { get; set; }
        /// <summary>
        /// Seha organization Id Level 2
        /// </summary>
        public int? SehaOrganizationIdLevel2 { get; set; }
        /// <summary>
        /// Region Id 
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// City Id 
        /// </summary>
        public int CityId { get; set; }

    }
    /// <summary>
    /// DP Request Info
    /// </summary>
    public class DPRequestInfo
    {
        /// <summary>
        /// Duration
        /// </summary>
        [Required]
        public int Duration { get; set; }
        /// <summary>
        /// Total Weekly Hours
        /// </summary>
        public decimal TotalWeeklyHours { get; set; }
    }
    /// <summary>
    /// Day Schedule Info
    /// </summary>
    public class DayScheduleInfo
    {
        /// <summary>
        /// From
        /// </summary>
        public DateTime? From { get; set; }
        /// <summary>
        /// To
        /// </summary>
        public DateTime? To { get; set; }
        /// <summary>
        /// Day
        /// </summary>
        public byte Day { get; set; }
        /// <summary>
        /// Total Hours
        /// </summary>
        public decimal TotalHours { get; set; }
    }
}

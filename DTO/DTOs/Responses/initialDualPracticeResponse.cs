using System;

namespace DTO.DTOs.Responses
{
    /// <summary>
    /// initial Dual Practice Response
    /// </summary>
    public class initialDualPracticeResponse
    {
        /// <summary>
        /// practitioner Info 
        /// </summary>
        public practitionerInfo practitionerInfo { get; set; }
        /// <summary>
        /// private Establishment Info
        /// </summary>
        public privateEstablishmentInfo privateEstablishmentInfo { get; set; }
        /// <summary>
        /// gov Establishment Info
        /// </summary>
        public govEstablishmentInfo govEstablishmentInfo { get; set; }
    }
    /// <summary>
    /// practitioner Info
    /// </summary>
    public class practitionerInfo
    {
        /// <summary>
        /// national Id
        /// </summary>
        public string nationalId { get; set; }
        /// <summary>
        /// full Name Ar
        /// </summary>
        public string fullNameAr { get; set; }
        /// <summary>
        /// full Name En
        /// </summary>
        public string fullNameEn { get; set; }
        /// <summary>
        /// nationality Ar
        /// </summary>
        public string nationalityAr { get; set; }
        /// <summary>
        /// nationality En
        /// </summary>
        public string nationalityEn { get; set; }
        /// <summary>
        /// speciality Code
        /// </summary>
        public string specialityCode { get; set; }
        /// <summary>
        /// speciality Ar
        /// </summary>
        public string specialityAr { get; set; }
        /// <summary>
        /// speciality En
        /// </summary>
        public string specialityEn { get; set; }
        /// <summary>
        /// scfhs Registration Number
        /// </summary>
        public string scfhsRegistrationNumber { get; set; }
        /// <summary>
        /// scfhs Registration Expiry Date
        /// </summary>
        public DateTime?  scfhsRegistrationExpiryDate { get; set; }
        /// <summary>
        /// license Number 
        /// </summary>
        public string licenseNumber { get; set; }
        /// <summary>
        /// license Expiry Date
        /// </summary>
        public DateTime? licenseExpiryDate { get; set; }
        /// <summary>
        /// gender
        /// </summary>
        public string gender_code { get; set; }
        /// <summary>
        /// date Of BirthH 
        /// </summary>
        public string birth_date_hijri { get; set; }
        /// <summary>
        /// date Of BirthG
        /// </summary>
        public DateTime birth_date_gregorian { get; set; }
        /// <summary>
        /// Scfhs Category Ar 
        /// </summary>
        public string ScfhsCategoryAr { get; set; }
        /// <summary>
        /// Scfhs Category En
        /// </summary>
        public string ScfhsCategoryEn { get; set; }
        /// <summary>
        /// scfhs Registration status
        /// </summary>
        public string scfhsRegistrationStatus { get; set; }

    }
    /// <summary>
    /// private Establishment Info 
    /// </summary>
    public class privateEstablishmentInfo
    {
        /// <summary>
        /// organization Id
        /// </summary>
        public int organizationId { get; set; }
        /// <summary>
        /// private User Id
        /// </summary>
        public int privateUserId { get; set; }
        /// <summary>
        /// name Ar 
        /// </summary>
        public string nameAr { get; set; }
        /// <summary>
        /// name En
        /// </summary>
        public string nameEn { get; set; }
        /// <summary>
        /// establishment Type Ar
        /// </summary>
        public string establishmentTypeAr { get; set; }
        /// <summary>
        /// establishment Type En
        /// </summary>
        public string establishmentTypeEn { get; set; }
        /// <summary>
        /// city Ar
        /// </summary>
        public string cityAr { get; set; }
        /// <summary>
        /// city En
        /// </summary>
        public string cityEn { get; set; }
        /// <summary>
        /// city Id
        /// </summary>
        public int cityId { get; set; }
        /// <summary>
        /// region Ar
        /// </summary>
        public string regionAr { get; set; }
        /// <summary>
        /// region En
        /// </summary>
        public string regionEn { get; set; }
        /// <summary>
        /// region Id
        /// </summary>
        public int regionId { get; set; }
        /// <summary>
        /// license Number
        /// </summary>
        public string licenseNumber { get; set; }
        /// <summary>
        /// license Expiry Date
        /// </summary>
        public DateTime? licenseExpiryDate { get; set; }
        /// <summary>
        /// administration NameAr
        /// </summary>
        public string administrationNameAr { get; set; }
        /// <summary>
        /// administration NameEn
        /// </summary>
        public string administrationNameEn { get; set; }
    }
    /// <summary>
    /// gov Establishment Info
    /// </summary>
    public class govEstablishmentInfo
    {
        /// <summary>
        /// Seha organization Id
        /// </summary>
        public int sehaOrganizationId { get; set; }
        /// <summary>
        /// name Ar
        /// </summary>
        public string nameAr { get; set; }
        /// <summary>
        /// name En
        /// </summary>
        public string nameEn { get; set; }
        /// <summary>
        /// facility Sector
        /// </summary>
        public string facilitySector { get; set; } // اسم القطاع الحكومي
        /// <summary>
        /// approval Level
        /// </summary>
        public int approvalLevel { get; set; }
        /// <summary>
        /// organization Id Level 2
        /// </summary>
        public int? sehaOrganizationIdLevel2 { get; set; } 
        /// <summary>
        /// cluster Id
        /// </summary>
        public string clusterId { get; set; }
        /// <summary>
        /// Management Area Id
        /// </summary>
        public int managementAreaId { get; set; }
        /// <summary>
        /// city Id
        /// </summary>
        public int cityId { get; set; }
        /// <summary>
        /// city En
        /// </summary>
        public string cityEn { get; set; }
        /// <summary>
        /// city Ar
        /// </summary>
        public string cityAr { get; set; }
        /// <summary>
        /// region Id
        /// </summary>
        public int regionId { get; set; }
    }
}

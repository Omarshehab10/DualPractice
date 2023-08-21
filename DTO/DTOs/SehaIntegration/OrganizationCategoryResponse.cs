using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.SehaIntegration
{
    /// <summary>
    /// Organization Category Response
    /// </summary>
    public class OrganizationCategoryResponse
    {
        /// <summary>
        /// Organization Id 
        /// </summary>
        public int OrganizationId { get; set; }
        /// <summary>
        /// Organization Name Ar 
        /// </summary>
        public string OrganizationNameAr { get; set; }
        /// <summary>
        /// Organization Name En
        /// </summary>
        public string OrganizationNameEn { get; set; }

        // OrganizationMedicalFacility table
        /// <summary>
        /// Category 
        /// </summary>
        public int Category { get; set; }

        // MedicalFacilityCategory table
        /// <summary>
        /// Sector Id 
        /// </summary>
        public int SectorId { get; set; }

        // MedicalFacilitySector table
        /// <summary>
        /// Medical Facility Sector Name Ar 
        /// </summary>
        public string MedicalFacilitySectorNameAr { get; set; }
        /// <summary>
        /// Medical Facility Sector Name En 
        /// </summary>
        public string MedicalFacilitySectorNameEn { get; set; }
        /// <summary>
        /// Service Code Prefix 
        /// </summary>
        public string ServiceCodePrefix { get; set; }
        /// <summary>
        /// City Id 
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// Region Id
        /// </summary>
        public int RegionId { get; set; }
    }
}

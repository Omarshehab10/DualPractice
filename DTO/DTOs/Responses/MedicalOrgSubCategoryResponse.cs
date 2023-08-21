using System.Collections.Generic;

namespace DTO.DTOs.Responses
{
    /// <summary>
    /// Medical Org Sub Category Response
    /// </summary>
    public class MedicalOrgSubCategoryResponse
    {
        /// <summary>
        /// Lookup
        /// </summary>
        public List<MedicalOrganizationSubCategory> Lookup { get; set; }
    }
    /// <summary>
    /// name En
    /// </summary>
    public class MedicalOrganizationSubCategory
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name Ar
        /// </summary>
        public string NameAr { get; set; }
        /// <summary>
        /// name En
        /// </summary>
        public string NameEn { get; set; }
        /// <summary>
        /// Category
        /// </summary>
        public MedicalOrganizationCategory Category { get; set; }
    }
    /// <summary>
    /// Medical Organization Category
    /// </summary>
    public class MedicalOrganizationCategory
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// name Ar
        /// </summary>
        public string NameAr { get; set; }
        /// <summary>
        /// name En
        /// </summary>
        public string NameEn { get; set; }
        /// <summary>
        /// Sector Id
        /// </summary>
        public int SectorId { get; set; }

    }
}

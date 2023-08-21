using System;

namespace DTO.DTOs.Responses
{
    public class OrganizationFullData
    {
            public int Id { get; set; }
            public int OrganizationId { get; set; }
            public string NameEn { get; set; }
            public string NameAr { get; set; }
            public int? NhciorganizationId { get; set; }
            public int? Mohid { get; set; }
            public int? OrganizationTypeId { get; set; }
            public int CityId { get; set; }
            public int RegionId { get; set; }
            public int? ClusterId { get; set; }
            public DateTime? CreateDate { get; set; }
            public DateTime? UpdateDate { get; set; }
            public string LicenseNumber { get; set; }
            public int? CreatedBy { get; set; }
            public int? UpdatedBy { get; set; }
            public string CityNameAr { get; set; }
            public string CityNameEn { get; set; }
            public string RegionNameAr { get; set; }
            public string RegionNameEn { get; set; }
            public int? CategoryId { get; set; }
            public int? SubcategoryId { get; set; }
            public int? SectorId { get; set; }
            public int? ManagmentAreaId { get; set; }
            public string ManagmentAreaAr { get; set; }
            public string ManagmentAreaEn { get; set; }
            public string SubcategoryNameAr { get; set; }
            public int? TypeFlag { get; set; }
            public string SubcategoryNameEn { get; set; }

    }
}

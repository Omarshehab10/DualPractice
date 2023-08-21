using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Models
{
    public partial class Organization
    {
        public Organization()
        {
            DpRequestPractionerMainOrg = new HashSet<DpRequest>();
            DpRequestRequestingOrg = new HashSet<DpRequest>();
            InverseSehaAdministrativeOrganization = new HashSet<Organization>();
        }

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
        public int? TypeFlag { get; set; }
        public int? SehaAdministrativeOrganizationId { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }

        public virtual City City { get; set; }
        public virtual AdministrativeArea ManagmentArea { get; set; }
        public virtual Region Region { get; set; }
        public virtual Organization SehaAdministrativeOrganization { get; set; }
        public virtual MedicalOrganizationSubCategory Subcategory { get; set; }
        public virtual AdminstrativeOrgLookup AdminstrativeOrgLookup { get; set; }
        public virtual ICollection<DpRequest> DpRequestPractionerMainOrg { get; set; }
        public virtual ICollection<DpRequest> DpRequestRequestingOrg { get; set; }
        public virtual ICollection<Organization> InverseSehaAdministrativeOrganization { get; set; }
    }
}

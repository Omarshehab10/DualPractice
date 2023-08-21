using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Models
{
    public partial class Practioner
    {
        public Practioner()
        {
            DpRequest = new HashSet<DpRequest>();
        }

        public int Id { get; set; }
        public int Nationality { get; set; }
        public string FirstNameAr { get; set; }
        public string SecondNameAr { get; set; }
        public string ThirdNameAr { get; set; }
        public string LastNameAr { get; set; }
        public string FirstNameEn { get; set; }
        public string SecondNameEn { get; set; }
        public string ThirdNameEn { get; set; }
        public string LastNameEn { get; set; }
        public string LicenseNumber { get; set; }
        public string SpecialtyCode { get; set; }
        public string SpecialtyNameAr { get; set; }
        public string SpecialtyNameEn { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string ScfhsRegistrationNumber { get; set; }
        public DateTime? ScfhsRegistrationExpiryDate { get; set; }
        public string DateOfBirthH { get; set; }
        public DateTime? DateOfBirthG { get; set; }
        public string Gender { get; set; }
        public string NationalId { get; set; }
        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
        public string ScfhsCategoryAr { get; set; }
        public string ScfhsCategoryEn { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual Nationality NationalityNavigation { get; set; }
        public virtual ICollection<DpRequest> DpRequest { get; set; }
    }
}

using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Models
{
    public partial class MedicalOrganizationSubCategory
    {
        public MedicalOrganizationSubCategory()
        {
            Organization = new HashSet<Organization>();
        }

        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public int? CategoryId { get; set; }

        public virtual ICollection<Organization> Organization { get; set; }
    }
}

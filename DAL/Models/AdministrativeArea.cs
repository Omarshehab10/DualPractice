using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Models
{
    public partial class AdministrativeArea
    {
        public AdministrativeArea()
        {
            Organization = new HashSet<Organization>();
        }

        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public int RegionId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatorUserId { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public int? LastModifierUserId { get; set; }

        public virtual ICollection<Organization> Organization { get; set; }
    }
}

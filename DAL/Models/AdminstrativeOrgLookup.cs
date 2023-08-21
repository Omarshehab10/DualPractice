using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Models
{
    public partial class AdminstrativeOrgLookup
    {
        public int Id { get; set; }
        public int SehaOrganizationId { get; set; }
        public int? RegistryId { get; set; }
        public bool IsCluster { get; set; }

        public virtual Organization SehaOrganization { get; set; }
    }
}

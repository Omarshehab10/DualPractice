using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Models
{
    public partial class User
    {
        public User()
        {
            Comment = new HashSet<Comment>();
            DpRequestCreatedByNavigation = new HashSet<DpRequest>();
            DpRequestUpdatedByNavigation = new HashSet<DpRequest>();
            Logs = new HashSet<Logs>();
            Practioner = new HashSet<Practioner>();
        }

        public int Id { get; set; }
        public int? OrganizationId { get; set; }
        public string IdiqamaNumber { get; set; }
        public string FullNameAr { get; set; }
        public string FullNameEn { get; set; }
        public int ExternalUserId { get; set; }
        public string UserPermissions { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<DpRequest> DpRequestCreatedByNavigation { get; set; }
        public virtual ICollection<DpRequest> DpRequestUpdatedByNavigation { get; set; }
        public virtual ICollection<Logs> Logs { get; set; }
        public virtual ICollection<Practioner> Practioner { get; set; }
    }
}

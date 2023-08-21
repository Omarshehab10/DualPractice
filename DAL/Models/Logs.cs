using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Models
{
    public partial class Logs
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime? Date { get; set; }
        public int? ActionType { get; set; }
        public string NormalizedServiceCode { get; set; }

        public virtual ActionTypeLookup ActionTypeNavigation { get; set; }
        public virtual User User { get; set; }
    }
}

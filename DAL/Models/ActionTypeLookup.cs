using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Models
{
    public partial class ActionTypeLookup
    {
        public ActionTypeLookup()
        {
            Logs = new HashSet<Logs>();
        }

        public int Id { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Logs> Logs { get; set; }
    }
}

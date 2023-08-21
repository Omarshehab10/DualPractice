using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class RejectionReasonsLookup
    {
        public RejectionReasonsLookup()
        {
            Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }
        public string ReasonAr { get; set; }
        public string ReasonEn { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}

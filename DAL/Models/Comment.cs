using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public partial class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreatedBy { get; set; }
        public int DprequestId { get; set; }
        public int? RejectionReasonId { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual DpRequest Dprequest { get; set; }
        public virtual RejectionReasonsLookup RejectionReason { get; set; }
    }
}

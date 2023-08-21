using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Models
{
    public partial class DpRequest
    {
        public DpRequest()
        {
            Comment = new HashSet<Comment>();
            DaySchedule = new HashSet<DaySchedule>();
            PaymentTransactionDetail = new HashSet<PaymentTransactionDetail>();
        }

        public int Id { get; set; }
        public int RequestingOrgId { get; set; }
        public int PractionerMainOrgId { get; set; }
        public int RequestStatus { get; set; }
        public int Duration { get; set; }
        public string ServiceCodePrefix { get; set; }
        public DateTime? ServiceCodeDate { get; set; }
        public int? ServiceCodeCode { get; set; }
        public string NormalizedServiceCode { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int ApprovalLevel { get; set; }
        public decimal? TotalWeeklyHours { get; set; }
        public string PractionerMobileNumber { get; set; }
        public string PractionerEmailAddress { get; set; }
        public string PractionerIdNumber { get; set; }
        public string ApprovedReportUrl { get; set; }
        public int? MohLevel2SehaOrgId { get; set; }

        public virtual User CreatedByNavigation { get; set; }
        public virtual Practioner PractionerIdNumberNavigation { get; set; }
        public virtual Organization PractionerMainOrg { get; set; }
        public virtual Organization RequestingOrg { get; set; }
        public virtual User UpdatedByNavigation { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<DaySchedule> DaySchedule { get; set; }
        public virtual ICollection<PaymentTransactionDetail> PaymentTransactionDetail { get; set; }
    }
}

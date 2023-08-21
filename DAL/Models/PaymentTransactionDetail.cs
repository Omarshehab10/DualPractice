using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Models
{
    public partial class PaymentTransactionDetail
    {
        public int Id { get; set; }
        public int DprequestId { get; set; }
        public int TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public string ServiceCode { get; set; }
        public int DpprocessId { get; set; }
        public int? DeductedPoints { get; set; }
        public int PaymentType { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual DpRequest Dprequest { get; set; }
    }
}

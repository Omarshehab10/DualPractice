using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Models
{
    public partial class DaySchedule
    {
        public int Id { get; set; }
        public int DprequestId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public byte Day { get; set; }
        public decimal? TotalHours { get; set; }
        public DateTime? From2 { get; set; }
        public DateTime? To2 { get; set; }

        public virtual DpRequest Dprequest { get; set; }
    }
}

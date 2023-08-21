using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.Responses
{
    /// <summary>
    ///  Organization Data
    /// </summary>
    public class RequestsOfPractionerMainOrgResponse
    {
        /// <summary>
        /// Request Normalized Service Code
        /// </summary>
        public string ReqServiceCode { get; set; }
        /// <summary>
        ///  National Id
        /// </summary>
        public string NationalId { get; set; }
        /// <summary>
        ///  Practioner Name
        /// </summary>
        public string PractionerName { get; set; }
        /// <summary>
        ///  Date Of Requset
        /// </summary>
        public string DateOfRequset { get; set; }
        /// <summary>
        ///  Total Weekly Hours
        /// </summary>
        public decimal TotalWeeklyHours { get; set; }
        /// <summary>
        ///  Resquest Status
        /// </summary>
        public int ResquestStatus { get; set; }
        /// <summary>
        ///  Requesting Org Id
        /// </summary>
        public int RequestingOrgId { get; set; }
        /// <summary>
        ///  Requesting Org Name
        /// </summary>
        public string RequestingOrgName { get; set; }
        /// <summary>
        ///  Approval End Date
        /// </summary>
        public DateTime? ApprovalEndDate { get; set; }
    }
}

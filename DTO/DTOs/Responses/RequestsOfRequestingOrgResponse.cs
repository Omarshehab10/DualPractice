using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.Responses
{
    /// <summary>
    ///  Requests Of Requesting Org Response
    /// </summary>
    public class RequestsOfRequestingOrgResponse
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
        ///  Resquest Status
        /// </summary>
        public int ResquestStatus { get; set; }
        /// <summary>
        ///  Practioner Main Org Name
        /// </summary>
        public int PractionerMainOrgId { get; set; }
        /// <summary>
        ///  Practioner Main Org Name
        /// </summary>
        public string PractionerMainOrgName { get; set; }
        /// <summary>
        ///  Approval End Date
        /// </summary>
        public DateTime? ApprovalEndDate { get; set; }
    }
}

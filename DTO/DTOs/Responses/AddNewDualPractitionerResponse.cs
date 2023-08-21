using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.Responses
{
    /// <summary>
    /// Add New Dual Practitioner Response 
    /// </summary>
    public class AddNewDualPractitionerResponse : DefaultModel
    {
        /// <summary>
        /// Request ID
        /// </summary>
        public string ReqServiceCode { get; set; }
    }
}

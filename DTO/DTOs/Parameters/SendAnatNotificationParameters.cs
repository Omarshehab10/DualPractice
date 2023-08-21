using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTOs.Parameters
{
    /// <summary>
    /// Send Anat Notification Parameters
    /// </summary>
    public class SendAnatNotificationParameters
    {
        /// <summary>
        /// Request Id (ServiceCode)
        /// </summary>
        [JsonProperty("request_id")]
        public string RequestId { get; set; }
        /// <summary>
        /// Practitioner Identification Number
        /// </summary>
        [JsonProperty("pract_identification_number")]
        public string PractNationalId { get; set; }
        /// <summary>
        /// Practitioner Full NameAr
        /// </summary>
        [JsonProperty("pract_fullname_ar")]
        public string PractFullNameAr { get; set; }
        /// <summary>
        /// Practitioner Full NameEn
        /// </summary>
        [JsonProperty("pract_fullname_en")]
        public string PractFullNameEn { get; set; }
    }
}

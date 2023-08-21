using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.Responses
{
    /// <summary>
    /// Notification To Anat Response
    /// </summary>
    public class NotificationToAnatResponse
    {
        /// <summary>
        /// Sent
        /// </summary>
        [JsonProperty("sent")]
        public bool Sent { get; set; }
    }
}

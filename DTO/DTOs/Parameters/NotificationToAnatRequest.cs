using Newtonsoft.Json;
using System.Collections.Generic;

namespace DTO.DTOs.Parameters
{
    /// <summary>
    /// Notification To Anat Request
    /// </summary>
    public class NotificationToAnatRequest
    {
        /// <summary>
        /// Service Name
        /// </summary>
        [JsonProperty("service_name")]
        public string ServiceName { get; set; }
        /// <summary>
        /// Title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>
        /// Body
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }
        /// <summary>
        /// Identification Number
        /// </summary>
        [JsonProperty("identification_number")]
        public List<string> NumberId { get; set; }
        /// <summary>
        /// Request Info
        /// </summary>
        [JsonProperty("data")]
        public RequestInfo requestInfo { get; set; }
        /// <summary>
        /// Send Anat SMS
        /// </summary>
        [JsonProperty("sms")]
        public SMS SendSms { get; set; }

    }
    /// <summary>
    /// Request Info
    /// </summary>
    public class RequestInfo
    {
        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
        /// <summary>
        /// Dual Practice
        /// </summary>
        [JsonProperty("dual_practice")]
        public DualPractice DualPractice { get; set; }
    }
    /// <summary>
    /// Dual Practice
    /// </summary>
    public class DualPractice
    {
        /// <summary>
        /// Request Id (ServiceCode)
        /// </summary>
        [JsonProperty("request_id")]
        public string RequestId { get; set; }
    }

    /// <summary>
    /// Send Anat SMS
    /// </summary>
    public class SMS
    {
        /// <summary>
        /// is SMS Sent
        /// </summary>
        [JsonProperty("is_Sent")]
        public bool IsSent { get; set; }

        /// <summary>
        /// SMS Body
        /// </summary>
        [JsonProperty("text")]
        public string SmsBody { get; set; }
    }
}

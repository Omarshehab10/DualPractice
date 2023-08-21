namespace DTO.DTOs.Parameters
{
    /// <summary>
    /// Inquire Balance Request
    /// </summary>
    public class InquireBalanceRequest
    {
        /// <summary>
        ///  service processid
        /// </summary>
        public string service_processid { set; get; }
        /// <summary>
        /// Seha access token
        /// </summary>
        public string access_token { set; get; }
    }
}

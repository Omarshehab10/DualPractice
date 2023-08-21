namespace DTO.DTOs.SehaIntegration
{
    /// <summary>
    /// Seha Inquire Balance Response
    /// </summary>
    public class InquireBalanceResponse
    {
        /// <summary>
        /// InquireBalanceResponse
        /// </summary>
        public InquireBalanceResponse()
        {
        }

        /// <summary>
        /// InquireBalanceResponse
        /// </summary>
        public InquireBalanceResponse(bool success, decimal valid_balance, string error_description, bool update_token)
        {
            this.success = success;
            this.valid_balance = valid_balance;
            this.error_description = error_description;
            this.update_token = update_token;
        }
        /// <summary>
        /// success
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// valid_balance
        /// </summary>
        public decimal? valid_balance { get; set; }
        /// <summary>
        /// error_description
        /// </summary>
        public string error_description { get; set; }
        /// <summary>
        /// update_token
        /// </summary>
        public bool? update_token { get; set; }
    }
}

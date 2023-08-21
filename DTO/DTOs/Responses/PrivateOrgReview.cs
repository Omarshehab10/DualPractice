namespace DTO.DTOs.Responses
{
    /// <summary>
    ///  Organization Data
    /// </summary>
    public class PrivateOrgReview
    {
        /// <summary>
        ///  Dprequest Id
        /// </summary>
        public int DprequestId { get; set; }
        /// <summary>
        ///  Accepted
        /// </summary>
        public bool Accepted { get; set; }
        /// <summary>
        ///  Transaction Id
        /// </summary>
        public int? TransactionId { get; set; }
        /// <summary>
        ///  Transaction Code
        /// </summary>
        public string? TransactionCode { get; set; }
        /// <summary>
        ///  Service Code
        /// </summary>
        public string ServiceCode { get; set; }
        /// <summary>
        ///  Dpprocess Id
        /// </summary>
        public int? DpprocessId { get; set; }
        /// <summary>
        ///  Deducted Points
        /// </summary>
        public int? DeductedPoints { get; set; }
        /// <summary>
        ///  Payment Type
        /// </summary>
        public int? PaymentType { get; set; }
    }
}

namespace DTO.Parameters.PractitionerInfo
{
    /// <summary>
    /// Practitioner Info Request
    /// </summary>
    public class PractitionerInfoRequest
    {
        /// <summary>
        /// National ID
        /// </summary>
        public string NationalID { get; set; }
        /// <summary>
        /// Date Of Birth
        /// </summary>
        public string DateOfBirth { get; set; }
        /// <summary>
        /// Is Gregorian?
        /// </summary>
        public bool IsGregorian { get; set; }
    }
}

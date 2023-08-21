namespace DTO.DTOs.OrganizationRegistry
{
    /// <summary>
    /// Organization Registry Response
    /// </summary>
    public class OrganizationRegistryResponse
    {
        /// <summary>
        /// Organization Data 
        /// </summary>
        public OrganizationData Data { get; set; }
        /// <summary>
        /// True -> have data, False -> no data
        /// </summary>
        public bool Ok { get; set; }
    }
    /// <summary>
    /// Organization Data 
    /// </summary>
    public class OrganizationData
    {
        /// <summary>
        /// Code
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// Moh Id
        /// </summary>
        public string moh_id { get; set; }
        /// <summary>
        /// Organization Id 
        /// </summary>
        public string organization_id { get; set; }
        /// <summary>
        /// License Number
        /// </summary>
        public string license_number { get; set; }
        /// <summary>
        /// license status
        /// </summary>
        public string license_status { get; set; }
        /// <summary>
        /// license expiry date
        /// </summary>
        public string license_expiry_date { get; set; }
        /// <summary>
        /// Name Arabic 
        /// </summary>
        public string name_ar { get; set; }
        /// <summary>
        /// NAme English
        /// </summary>
        public string name_en { get; set; }
        /// <summary>
        /// Seha Id
        /// </summary>
        public string seha_id { get; set; }
        /// <summary>
        /// Beds Count
        /// </summary>
        public string beds_count { get; set; }
        /// <summary>
        /// Long Itude
        /// </summary>
        public string longitude { get; set; }
        /// <summary>
        /// lat itude
        /// </summary>
        public string latitude { get; set; }
        /// <summary>
        /// level Of Care
        /// </summary>
        public string level_of_care { get; set; }
        /// <summary>
        /// type of care
        /// </summary>
        public string type_of_care { get; set; }
        /// <summary>
        /// type of care code
        /// </summary>
        public string type_of_care_code { get; set; }
        /// <summary>
        /// health directory arabic
        /// </summary>
        public string health_directory_ar { get; set; }
        /// <summary>
        /// health directory english
        /// </summary>
        public string health_directory_en { get; set; }
        /// <summary>
        /// health directory seha id
        /// </summary>
        public string health_directory_seha_id { get; set; }
        /// <summary>
        /// sector arabic
        /// </summary>
        public string sector_ar { get; set; }
        /// <summary>
        /// sector english
        /// </summary>
        public string sector_en { get; set; }

        /// <summary>
        /// sector identifier
        /// </summary>
        public string sector_identifier { get; set; }
        /// <summary>
        /// city arabic
        /// </summary>
        public string city_ar { get; set; }
        /// <summary>
        /// city english
        /// </summary>
        public string city_en { get; set; }
        /// <summary>
        /// city code
        /// </summary>
        public string city_code { get; set; }
        /// <summary>
        /// region arabic
        /// </summary>
        public string region_ar { get; set; }
        /// <summary>
        /// region english
        /// </summary>
        public string region_en { get; set; }
        /// <summary>
        /// region code
        /// </summary>
        public string region_code { get; set; }
        /// <summary>
        /// cluster code
        /// </summary>
        public string cluster_code { get; set; }
        /// <summary>
        /// cluster name arabic
        /// </summary>
        public string cluster_name_ar { get; set; }
        /// <summary>
        /// cluster name english
        /// </summary>
        public string cluster_name_en { get; set; }
    }
}

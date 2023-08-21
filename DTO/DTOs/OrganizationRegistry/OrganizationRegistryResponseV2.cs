namespace DTO.DTOs.OrganizationRegistry
{
    /// <summary>
    /// Organization Registry V2 Response 
    /// </summary>
    public class OrganizationRegistryV2Response
    {
        /// <summary>
        /// Organization Data V2
        /// </summary>
        public OrganizationDataV2 Data { get; set; }
        /// <summary>
        /// True -> have data, False -> no data
        /// </summary>
        public bool Ok { get; set; }
    }
    /// <summary>
    /// Organization Data 
    /// </summary>
    public class OrganizationDataV2
    {
        /// <summary>
        /// facility address
        /// </summary>
        public string facility_address { get; set; }
        /// <summary>
        /// postal address
        /// </summary>
        public string postal_address { get; set; }
        /// <summary>
        ///  organization name ar
        /// </summary>
        public string organization_name_ar { get; set; }
        /// <summary>
        /// organization name en
        /// </summary>
        public string organization_name_en { get; set; }
        /// <summary>
        /// organization id 
        /// </summary>
        public string organization_id { get; set; }
        /// <summary>
        /// organization speciality
        /// </summary>
        public string organization_speciality { get; set; }
        /// <summary>
        /// organization credentials
        /// </summary>
        public string organization_credentials { get; set; }
        /// <summary>
        /// organization contact
        /// </summary>
        public string organization_contact { get; set; }
        /// <summary>
        /// facility unique identifier
        /// </summary>
        public string facility_unique_identifier { get; set; }
        /// <summary>
        /// facility sector identifier
        /// </summary>
        public string facility_sector_identifier { get; set; }
        /// <summary>
        /// facility level of care identifier
        /// </summary>
        public string facility_level_of_care_identifier { get; set; }
        /// <summary>
        /// facility type of care
        /// </summary>
        public string facility_type_of_care { get; set; }
        /// <summary>
        /// facility operation status
        /// </summary>
        public string facility_operation_status { get; set; }
        /// <summary>
        /// electronic service url
        /// </summary>
        public string electronic_service_url { get; set; }
        /// <summary>
        /// medical records delivery email
        /// </summary>
        public string medical_records_delivery_email { get; set; }
        /// <summary>
        /// last updated time
        /// </summary>
        public string last_updated_time { get; set; }
        /// <summary>
        /// provider language supported
        /// </summary>
        public string provider_language_supported { get; set; }
        /// <summary>
        /// provider relationship
        /// </summary>
        public string provider_relationship { get; set; }
        /// <summary>
        /// available hospital beds admitted Patients
        /// </summary>
        public string available_hospital_beds_admitted_Patients { get; set; }
        /// <summary>
        /// available operating rooms
        /// </summary>
        public string ciavailable_operating_roomsy_en { get; set; }
        /// <summary>
        /// available emergency beds
        /// </summary>
        public string available_emergency_beds { get; set; }
        /// <summary>
        /// available intensive care unit areas average beds
        /// </summary>
        public string available_intensive_care_unit_areas_average_beds { get; set; }
        /// <summary>
        /// total hospital beds
        /// </summary>
        public string total_hospital_beds { get; set; }
        /// <summary>
        /// teaching status
        /// </summary>
        public string teaching_status { get; set; }
        /// <summary>
        /// the 700 number
        /// </summary>
        public string the_700_number { get; set; }
    }
}

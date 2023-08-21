using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.PractitionersRegistry
{
    /// <summary>
    /// Practitioner Registry Response 
    /// </summary>
    public class PractitionerRegistryResponseV1
    {  
        /// <summary>
        /// Data of typ
        /// </summary>
        public PractitionerDataV3 Data { get; set; }
        /// <summary>
        /// True -> have data, False -> no data
        /// </summary>
        public bool Ok { get; set; }
    }
    /// <summary>
    /// Practitioner Data V3
    /// </summary>
    public class PractitionerDataV3
    {
        /// <summary>
        /// birth date gregorian
        /// </summary>
        public DateTime? birth_date_gregorian { get; set; }
        /// <summary>
        /// Pbirth date hijri
        /// </summary>
        public string birth_date_hijri { get; set; }
        /// <summary>
        /// first name ar
        /// </summary>
        public string first_name_ar { get; set; }
        /// <summary>
        /// first name en
        /// </summary>
        public string first_name_en { get; set; }
        /// <summary>
        /// full name ar
        /// </summary>
        public string full_name_ar { get; set; }
        /// <summary>
        /// full name en
        /// </summary>
        public string full_name_en { get; set; }
        /// <summary>
        /// gender ar
        /// </summary>
        public string gender_ar { get; set; }
        /// <summary>
        /// Practitioner Data
        /// </summary>
        public string gender_code { get; set; }
        /// <summary>
        /// gender code
        /// </summary>
        public string gender_en { get; set; }
        /// <summary>
        /// health id
        /// </summary>
        public string health_id { get; set; }
        /// <summary>
        /// hls licenses
        /// </summary>
        public List<HlsLicense> hls_licenses { get; set; }
        /// <summary>
        /// gov organizations
        /// </summary>
        public List<GovOrganization> gov_organizations { get; set; }
        /// <summary>
        /// id number
        /// </summary>
        public string id_number { get; set; }
        /// <summary>
        /// id type
        /// </summary>
        public string id_type { get; set; }
        /// <summary>
        /// last name ar
        /// </summary>
        public string last_name_ar { get; set; }
        /// <summary>
        /// last name en
        /// </summary>
        public string last_name_en { get; set; }
        /// <summary>
        /// nationality ar
        /// </summary>
        public string nationality_ar { get; set; }
        /// <summary>
        /// nationality code
        /// </summary>
        public string nationality_code { get; set; }
        /// <summary>
        /// nationality en
        /// </summary>
        public string nationality_en { get; set; }
        /// <summary>
        /// practitioner status
        /// </summary>
        public string practitioner_status { get; set; }
        /// <summary>
        /// religion
        /// </summary>
        public string religion { get; set; }
        /// <summary>
        /// scfhs
        /// </summary>
        public Scfhs scfhs { get; set; }
        /// <summary>
        /// second name ar
        /// </summary>
        public string second_name_ar { get; set; }
        /// <summary>
        /// second name en
        /// </summary>
        public string second_name_en { get; set; }
    }
    /// <summary>
    /// GovOrganization
    /// </summary>
    public class GovOrganization
    {
        /// <summary>
        /// employer org id
        /// </summary>
        public string employer_org_id { get; set; }
        /// <summary>
        /// employer org name ar
        /// </summary>
        public string employer_org_name_ar { get; set; }
        /// <summary>
        /// employment from
        /// </summary>
        public string employment_from { get; set; }
        /// <summary>
        /// employment to
        /// </summary>
        public string employment_to { get; set; }
    }

    /// <summary>
    /// HlsLicense
    /// </summary>
    public class HlsLicense
    {
        /// <summary>
        /// establishment name
        /// </summary>
        public string establishment_name { get; set; }
        /// <summary>
        /// establishment org id
        /// </summary>
        public string establishment_org_id { get; set; }
        /// <summary>
        /// expiration status
        /// </summary>
        public string expiration_status { get; set; }
        /// <summary>
        /// field code
        /// </summary>
        public string field_code { get; set; }
        /// <summary>
        /// field name ar
        /// </summary>
        public string field_name_ar { get; set; }
        /// <summary>
        /// field name en
        /// </summary>
        public string field_name_en { get; set; }
        /// <summary>
        /// license expiry date
        /// </summary>
        public DateTime? license_expiry_date { get; set; }
        /// <summary>
        /// license issue date
        /// </summary>
        public DateTime? license_issue_date { get; set; }
        /// <summary>
        /// license number
        /// </summary>
        public string license_number { get; set; }
        /// <summary>
        /// rank code
        /// </summary>
        public string rank_code { get; set; }
        /// <summary>
        /// rank name ar
        /// </summary>
        public string rank_name_ar { get; set; }
        /// <summary>
        /// rank name en
        /// </summary>
        public string rank_name_en { get; set; }
        /// <summary>
        /// specialty ar
        /// </summary>
        public string specialty_ar { get; set; }
        /// <summary>
        /// specialty code
        /// </summary>
        public string specialty_code { get; set; }
        /// <summary>
        /// specialty en
        /// </summary>
        public string specialty_en { get; set; }
        /// <summary>
        /// sub specialities list
        /// </summary>
        public List<SubSpecialitiesList> sub_specialities_list { get; set; }
    }
    /// <summary>
    /// Scfhs
    /// </summary>
    public class Scfhs
    {
        /// <summary>
        /// field code
        /// </summary>
        public string field_code { get; set; }
        /// <summary>
        /// field name ar
        /// </summary>
        public string field_name_ar { get; set; }
        /// <summary>
        /// field name en
        /// </summary>
        public string field_name_en { get; set; }
        /// <summary>
        /// license expiry date
        /// </summary>
        public DateTime? license_expiry_date { get; set; }
        /// <summary>
        /// license issue date
        /// </summary>
        public DateTime? license_issue_date { get; set; }
        /// <summary>
        /// license restrictions list
        /// </summary>
        public List<object> license_restrictions_list { get; set; }
        /// <summary>
        /// license status code
        /// </summary>
        public string license_status_code { get; set; }
        /// <summary>
        /// license status desc ar
        /// </summary>
        public string license_status_desc_ar { get; set; }
        /// <summary>
        /// license status desc en
        /// </summary>
        public string license_status_desc_en { get; set; }
        /// <summary>
        /// rank code
        /// </summary>
        public string rank_code { get; set; }
        /// <summary>
        /// rank name ar
        /// </summary>
        public string rank_name_ar { get; set; }
        /// <summary>
        /// rank name en
        /// </summary>
        public string rank_name_en { get; set; }
        /// <summary>
        /// registration number
        /// </summary>
        public string registration_number { get; set; }
        /// <summary>
        /// specialty code
        /// </summary>
        public string specialty_code { get; set; }
        /// <summary>
        /// specialty name ar
        /// </summary>
        public string specialty_name_ar { get; set; }
        /// <summary>
        /// specialty name en
        /// </summary>
        public string specialty_name_en { get; set; }
        /// <summary>
        /// status code
        /// </summary>
        public string status_code { get; set; }
        /// <summary>
        /// status desc ar
        /// </summary>
        public string status_desc_ar { get; set; }
        /// <summary>
        /// status desc en
        /// </summary>
        public string status_desc_en { get; set; }
        /// <summary>
        /// sub specialities list
        /// </summary>
        public List<SubSpecialitiesList> sub_specialities_list { get; set; }
    }
    /// <summary>
    /// SubSpecialitiesList
    /// </summary>
    public class SubSpecialitiesList
    {
        /// <summary>
        /// sub specialty code
        /// </summary>
        public string sub_specialty_code { get; set; }
        /// <summary>
        /// sub specialty name ar
        /// </summary>
        public string sub_specialty_name_ar { get; set; }
        /// <summary>
        /// sub specialty name en
        /// </summary>
        public string sub_specialty_name_en { get; set; }
    }
}



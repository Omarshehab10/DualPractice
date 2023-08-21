using System;
using System.Collections.Generic;

namespace DTO.DTOs.Responses
{
    /// <summary>
    /// Organization Data
    /// </summary>
    public class OrganizationData
    {
        /// <summary>
        ///  Organization Data
        /// </summary>
        public OrganizationData()
        {
        }
        /// <summary>
        /// Organization Data
        /// </summary>
        public OrganizationData(string _orgId, string _name_ar, string _name_en, int _type_flag, int _account_id, int _account_type_flag, string _license_number)
        {
            orgId = _orgId;
            name_ar = _name_ar;
            name_en = _name_en;
            type_flag = _type_flag;
            account_id = _account_id;
            account_type_flag = _account_type_flag;
            license_number = _license_number;
        }
        /// <summary>
        /// Organization Data
        /// </summary>
        public OrganizationData(string _orgId, string _name_ar, string _name_en, int _type_flag, int _account_id, int _account_type_flag, string _license_number, string categoryId, string subCategoryId, string crNumber, int subsectorId)
        {
            orgId = _orgId;
            name_ar = _name_ar;
            name_en = _name_en;
            type_flag = _type_flag;
            category_id = categoryId;
            subcategory_id = subCategoryId;
            account_id = _account_id;
            account_type_flag = _account_type_flag;
            license_number = _license_number;
            cr_number = crNumber;
            sub_sector_Id = subsectorId;
        }
        /// <summary>
        /// category id
        /// </summary>
        public string category_id { get; set; }
        /// <summary>
        /// subcategory id
        /// </summary>
        public string subcategory_id { get; set; }
        /// <summary>
        /// cr number
        /// </summary>
        public string cr_number { get; set; }
        /// <summary>
        /// org Id
        /// </summary>
        public string orgId { get; set; }
        /// <summary>
        /// name ar
        /// </summary>
        public string name_ar { get; set; }
        /// <summary>
        /// name en
        /// </summary>
        public string name_en { get; set; }
        /// <summary>
        /// type flag
        /// </summary>
        public int type_flag { get; set; }
        /// <summary>
        /// account id
        /// </summary>
        public int account_id { get; set; }
        /// <summary>
        /// account type flag
        /// </summary>
        public int account_type_flag { get; set; }
        /// <summary>
        /// city name ar
        /// </summary>
        public string city_name_ar { get; set; }
        /// <summary>
        /// city name en
        /// </summary>
        public string city_name_en { get; set; }
        /// <summary>
        /// region name ar
        /// </summary>
        public string region_name_ar { get; set; }
        /// <summary>
        /// region name en
        /// </summary>
        public string region_name_en { get; set; }
        /// <summary>
        /// city id
        /// </summary>
        public int city_id { get; set; }
        /// <summary>
        /// region id
        /// </summary>
        public int region_id { get; set; }
        /// <summary>
        /// cities of regins
        /// </summary>
        public Dictionary<int, int[]> cities_of_regions { get; set; }
        /// <summary>
        /// category id
        /// </summary>
        public Dictionary<int, int[]> cities_of_regins { get; set; }
        /// <summary>
        /// Citeis Regions
        /// </summary>
        public List<Citeis_RegionsObject> Citeis_Regions { get; set; }
        /// <summary>
        /// license number
        /// </summary>
        public string license_number { get; set; }
        /// <summary>
        /// license expiry date
        /// </summary>
        public DateTime? license_expiry_date { get; set; }
        /// <summary>
        /// short name ar
        /// </summary>
        public string short_name_ar { get; set; }
        /// <summary>
        /// short name en
        /// </summary>
        public string short_name_en { get; set; }
        /// <summary>
        /// handred number
        /// </summary>
        public string handred_number { get; set; }
        /// <summary>
        /// cr issue date
        /// </summary>
        public DateTime cr_issue_date { get; set; }
        /// <summary>
        /// sector
        /// </summary>
        public int sector { get; set; }
        /// <summary>
        /// managment area id
        /// </summary>
        public int managment_area_id { get; set; }
        /// <summary>
        /// LNOHM
        /// </summary>
        public string LNOHM { get; set; }
        /// <summary>
        /// sub sector Id
        /// </summary>
        public int sub_sector_Id { get; set; }
        /// <summary>
        /// estableshment type id
        /// </summary>
        public int estableshment_type_id { get; set; }
        /// <summary>
        /// owner number
        /// </summary>
        public string owner_number { get; set; }
    }

    /// <summary>
    /// Citeis Regions Object
    /// </summary>
    public class Citeis_RegionsObject
    {
        /// <summary>
        /// Region Id
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// Region Name Ar
        /// </summary>
        public string RegionNameAr { get; set; }
        /// <summary>
        /// Region Name En
        /// </summary>
        public string RegionNameEn { get; set; }
        /// <summary>
        /// City Id
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// City Name Ar
        /// </summary>
        public string CityNameAr { get; set; }
        /// <summary>
        /// City Name En
        /// </summary>
        public string CityNameEn { get; set; }

    }
    /// <summary>
    /// Organization Data Response
    /// </summary>
    public class OrganizationDataResponse
    {
        /// <summary>
        /// Org
        /// </summary>
        public OrganizationData Org { get; set; }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace DTO.DTOs.EstablishmentHLS
{
    /// <summary>
    /// Hls Response
    /// </summary>
    public class HlsResponse
    {
        /// <summary>
        /// Status
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        /// <summary>
        /// Data
        /// </summary>
        [JsonProperty(PropertyName = "establishment")]
        public Establishment Data { get; set; }

    }

    /// <summary>
    /// Establishment
    /// </summary>
    public class Establishment
    {
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Cr Number
        /// </summary>
        [JsonProperty(PropertyName = "cr_number")]
        public string CrNumber { get; set; }
        /// <summary>
        /// Practitioners Count
        /// </summary>
        [JsonProperty(PropertyName = "practitioners_count")]
        public int? PractitionersCount { get; set; }
        /// <summary>
        /// License
        /// </summary>
        [JsonProperty(PropertyName = "license")]
        public EstablishmentLicense License { get; set; }

    }
    /// <summary>
    /// Establishment License
    /// </summary>
    public class EstablishmentLicense
    {
        /// <summary>
        /// Legacy License Number
        /// </summary>
        [JsonProperty(PropertyName = "legacy_license_number")]
        public string LegacyLicenseNumber { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        /// <summary>
        /// Issue Date
        /// </summary>
        [JsonProperty(PropertyName = "issue_date")]
        public string IssueDate { get; set; }
        /// <summary>
        /// Expiry Date
        /// </summary>
        [JsonProperty(PropertyName = "expiry_date")]
        public string ExpiryDate { get; set; }
        /// <summary>
        /// Directorate
        /// </summary>
        [JsonProperty(PropertyName = "directorate")]
        public EstablishmentDirectorate Directorate { get; set; }
        /// <summary>
        /// Speciality
        /// </summary>
        [JsonProperty(PropertyName = "speciality")]
        public EstablishmentSpeciality Speciality { get; set; }
        /// <summary>
        /// specialities
        /// </summary>
        [JsonProperty(PropertyName = "specialities")]
        public List<EstablishmentSpeciality> specialities { get; set; }
        /// <summary>
        /// Type
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public EstablishmentType Type { get; set; }
    }
    /// <summary>
    /// Establishment Directorate
    /// </summary>
    public class EstablishmentDirectorate
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        /// <summary>
        /// Short Name
        /// </summary>
        [JsonProperty(PropertyName = "short_name")]
        public string ShortName { get; set; }
    }
    /// <summary>
    /// Establishment Speciality
    /// </summary>
    public class EstablishmentSpeciality
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
    /// <summary>
    /// Establishment Type
    /// </summary>
    public class EstablishmentType
    {
        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}

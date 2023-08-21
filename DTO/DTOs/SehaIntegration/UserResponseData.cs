using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.SehaIntegration
{
    /// <summary>
    /// User Response Data
    /// </summary>
    public class UserResponseData
    {
        /// <summary>
        /// User Id 
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// ID/Iqama Number 
        /// </summary>
        public string IDIqamaNumber { get; set; }
        /// <summary>
        /// Name Ar 
        /// </summary>
        public string NameAr { get; set; }
        /// <summary>
        /// Name En 
        /// </summary>
        public string NameEn { get; set; }
        /// <summary>
        /// Nationality Id 
        /// </summary>
        public int NationalityId { get; set; }
        /// <summary>
        /// Nationality Name Ar 
        /// </summary>
        public string NationalityNameAr { get; set; }
        /// <summary>
        /// Nationality Name En 
        /// </summary>
        public string NationalityNameEn { get; set; }
        /// <summary>
        /// Is Saudi 
        /// </summary>
        public int IsSaudi { get; set; }
        /// <summary>
        /// Date Of Birth 
        /// </summary>
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// Mobile Number 
        /// </summary>
        public string MobileNumber { get; set; }
        /// <summary>
        /// Email Address 
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Username 
        /// </summary>
        public string UserName { get; set; }
    }
}

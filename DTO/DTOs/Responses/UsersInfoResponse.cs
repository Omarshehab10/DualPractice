using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.Responses
{
    /// <summary>
    /// Data
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Full Name
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Service Permissions
        /// </summary>
        public string ServicePermissions { get; set; }
        /// <summary>
        /// Organization Id
        /// </summary>
        public int OrganizationId { get; set; }
        /// <summary>
        /// Organization Name
        /// </summary>
        public string OrganizationName { get; set; }
        /// <summary>
        /// Permission Id
        /// </summary>
        public int PermissionId { get; set; }
        /// <summary>
        /// Id Iqama Number
        /// </summary>
        public string IdIqamaNumber { get; set; }
    }
    /// <summary>
    /// Users Info Response
    /// </summary>
    public class UsersInfoResponse
    {
        /// <summary>
        /// Data
        /// </summary>
        public List<Data> Data { get; set; }
        /// <summary>
        /// Error Code
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// Error Message
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}

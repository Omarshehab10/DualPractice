using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.Parameters
{
    /// <summary>
    /// Users Info Request
    /// </summary>
    public class UsersInfoRequest
    {
        /// <summary>
        /// Organization Id
        /// </summary>
        public int OrganizationId { get; set; }
        /// <summary>
        /// Organization Id
        /// </summary>
        public int ServiceId { get; set; }
        /// <summary>
        /// Permission Id
        /// </summary>
        public int PermissionId { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.Parameters
{
    /// <summary>
    /// Get Users Info Request
    /// </summary>
    public class GetUsersInfoRequest
    {
        /// <summary>
        /// Data
        /// </summary>
        public UsersInfoRequest Data { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
    }
}

using Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.Parameters
{
    public class AddLogRequest
    {
        public int UserId { get; set; }
        public AddLogs? ActionType { get; set; }
        public string NormalizedServiceCode { get; set; }
    }
}

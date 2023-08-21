using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.DTOs.EmailSetting
{
    public class EmailSettingsReq
    {
        public string EmailToId { get; set; }
        public string EmailToName { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
    }
}

using DTO.DTOs.EmailSetting;
using DTO.DTOs.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.ExternalServices.SendEmailService
{
    public interface IEmailService
    {
        Task SendMailOrg(UsersInfoRequest usersInfoRequest, EmailContent emailContent);
        Task SendEmail(EmailSettingsReq emailSettingsReq);
    }
}

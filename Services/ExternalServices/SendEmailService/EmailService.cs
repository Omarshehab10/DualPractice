using Common.Localization;
using Common.Types;
using DTO.DTOs.EmailSetting;
using DTO.DTOs.Parameters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Common;
using Services.ExternalServices.SehaEndPoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Services.ExternalServices.SendEmailService
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IAppResourceService _resourceService;
        private readonly ICommonService _commonService;
        private readonly ISehaService _sehaService;

        EmailSettings _emailSettings = null;
        public EmailService(IOptions<EmailSettings> options, 
            ILogger<EmailService> logger, IAppResourceService resourceService,
            ISehaService sehaService, ICommonService commonService)
        {
            _emailSettings = options.Value;
            _logger = logger;
            _resourceService = resourceService;
            _commonService = commonService;
            _sehaService = sehaService;
        }

        public async Task SendMailOrg(UsersInfoRequest usersInfoRequest, EmailContent emailContent)
        {
            try
            {
                var OrgUsersData = await _sehaService.GetUsersInfo(usersInfoRequest);
                var EmailBodyOrg = "";
                var EmailGreating = "";

                foreach (var item in OrgUsersData.Data)
                {
                    EmailGreating = $"عزيزى / {item.FullName}";
                    EmailBodyOrg = string.Format(emailContent.Contents, emailContent.EmailSubjectOrg, EmailGreating, emailContent.EmailBody);
                    EmailSettingsReq emailSettingsReq = new EmailSettingsReq
                    {
                        EmailToId = item.Email,
                        EmailToName = item.FullName,
                        EmailSubject = emailContent.EmailSubjectOrg,
                        EmailBody = EmailBodyOrg
                    };
                    _logger.LogInformation("********* Get email Settings Req EmailToId Of the User to SendMail = (" + emailSettingsReq.EmailToId + ")", emailSettingsReq);
                    await SendEmail(emailSettingsReq);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"SendMailOrg - A problem occurred while sending the email to the user-  {e.Message}");
                //throw new CustomHttpException(HttpStatusCode.BadRequest, _resourceService.GetResource("Email_Not_Send"));
            }
        }

        public async Task SendEmail(EmailSettingsReq emailSettingsReqS)
        {
            try
            {
                _logger.LogInformation("********* Start Finish Sending Mail = (" + _emailSettings.Name + ")", _emailSettings.Name);
                var Host = _emailSettings.Host;
                var Port = _emailSettings.Port;
                var SenderName = _emailSettings.Name;
                var SenderEmail = _emailSettings.EmailId;
                var Password = _emailSettings.Password;
                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(Host, Port);
                smtpClient.Credentials = new NetworkCredential(SenderEmail, Password);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                AlternateView View = AlternateView.CreateAlternateViewFromString(emailSettingsReqS.EmailBody, null, MediaTypeNames.Text.Html);
                _logger.LogInformation("********* Get email Settings Req EmailFromId Of the User to SendMail = (" + _emailSettings.EmailId + ")", _emailSettings.EmailId);
                LinkedResource MOHLogo = new LinkedResource(_commonService.MapPath(@"StaticFiles/Common/logos.png"), MediaTypeNames.Image.Jpeg);
                MOHLogo.ContentId = "logos";
                MOHLogo.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                View.LinkedResources.Add(MOHLogo);
                LinkedResource SehaLogo = new LinkedResource(_commonService.MapPath(@"StaticFiles/Common/seha-logo.png"), MediaTypeNames.Image.Jpeg);
                SehaLogo.ContentId = "seha_logo";
                SehaLogo.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                View.LinkedResources.Add(SehaLogo);
                LinkedResource Twitter = new LinkedResource(_commonService.MapPath(@"StaticFiles/Common/twitter.png"), MediaTypeNames.Image.Jpeg);
                Twitter.ContentId = "twitter";
                Twitter.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                View.LinkedResources.Add(Twitter);
                LinkedResource BxLeftArrowAlt = new LinkedResource(_commonService.MapPath(@"StaticFiles/Common/bx-left-arrow-alt.png"), MediaTypeNames.Image.Jpeg);
                BxLeftArrowAlt.ContentId = "bx-left-arrow-alt";
                BxLeftArrowAlt.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                View.LinkedResources.Add(BxLeftArrowAlt);
                LinkedResource Phone = new LinkedResource(_commonService.MapPath(@"StaticFiles/Common/phone.png"), MediaTypeNames.Image.Jpeg);
                Phone.ContentId = "phone";
                Phone.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                View.LinkedResources.Add(Phone);
                mail.AlternateViews.Add(View);
                _logger.LogInformation("********* Mail Before Sent = (" + mail.Body + ")", mail.Body);
                mail.From = new MailAddress(SenderEmail, SenderName);
                mail.To.Add(new MailAddress(emailSettingsReqS.EmailToId));
                mail.Subject = emailSettingsReqS.EmailSubject;
                await smtpClient.SendMailAsync(mail);
                _logger.LogInformation("********* Mail Sent = (" + mail.Body + ")", mail.Body);
            }
            catch (Exception e)
            {
                _logger.LogError($"SendMail - A problem occurred while Finish sending the email to the user-  {e.Message}");
            }
        }

    }
}

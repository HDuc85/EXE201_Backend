using Data.ViewModel.Helper;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;

namespace Service.Helper.Email
{
    public class EmailHelper : IEmailHelper
    {
        EmailConfig _emailConfig;

        public EmailHelper(IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }
        public async Task SendEmail(EmailRequest emailRequest)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(_emailConfig.Provider, _emailConfig.Port);
                smtpClient.Credentials = new NetworkCredential(_emailConfig.Receiver, _emailConfig.Password);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_emailConfig.Receiver);
                mailMessage.To.Add(emailRequest.To);
                mailMessage.Subject = emailRequest.Subject;
                mailMessage.Body = emailRequest.Content;

                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.IsBodyHtml = true;
                if (emailRequest.AttachmentFilePaths.Length > 0)
                {
                    foreach (var path in emailRequest.AttachmentFilePaths)
                    {
                        Attachment attachment = new Attachment(path);
                        mailMessage.Attachments.Add(attachment);
                    }
                }

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



    }
}

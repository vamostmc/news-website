using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Web1.Models;

namespace Web1.Service
{
    public class SendMailService : ISendMailService
    {
        private readonly MailSetting _mailSettings;

        public SendMailService(IOptions<MailSetting> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task<Success> SetUpSendMail(string to, string subject, string body)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
                emailMessage.To.Add(new MailboxAddress("", to)); // Địa chỉ người nhận
                emailMessage.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                emailMessage.Body = bodyBuilder.ToMessageBody();

                using (var smtpClient = new SmtpClient())
                {
                    // Kết nối và xác thực
                    await smtpClient.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                    await smtpClient.SendAsync(emailMessage); // Gửi email
                    await smtpClient.DisconnectAsync(true); // Ngắt kết nối
                }
                return new Success { success = true };
            }
            catch (Exception ex) 
            {
                return new Success { success = false, message = ex.Message };
            }
            
        }

        

        

    }
}

using EzioLearning.Api.Models.Auth;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace EzioLearning.Api.Services
{
    public class MailService(MailSettings mailSettings)
    {

        public async Task SendMail(MailContent content)
        {
            var email = new MimeMessage();

            email.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            email.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            email.To.Add(MailboxAddress.Parse(content.To));
            
            email.Subject = content.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = content.HtmlBody
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();


            await smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);
            await smtp.SendAsync(email);


            await smtp.DisconnectAsync(true);

        }
    }
}

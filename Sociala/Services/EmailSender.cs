using System.Net.Mail;
using System.Net;

namespace EmailSendertServices
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("sociala421@gmail.com", "phaiplejngyycteb")
            };

            return client.SendMailAsync(
                new MailMessage(from: "sociala421@gmail.com",
                                to: email,
                                subject,
                                message
                                )
                {
                    IsBodyHtml = true 
                });
        }
    }
}
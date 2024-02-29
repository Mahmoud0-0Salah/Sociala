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
            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("M4_0_0_@outlook.com", "MMaasm4@")
            };

            return client.SendMailAsync(
                new MailMessage(from: "M4_0_0_@outlook.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}

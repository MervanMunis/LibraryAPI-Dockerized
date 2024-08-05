using LibraryAPI.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace LibraryAPI.Services.impl
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
            {
                Port = int.Parse(emailSettings["SmtpPort"]),
                Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["SenderEmail"], emailSettings["SenderName"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}

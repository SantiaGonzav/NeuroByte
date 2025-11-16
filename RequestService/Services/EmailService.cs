using System.Net;
using System.Net.Mail;

namespace RequestService.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await SendEmailToManyAsync(new[] { to }, subject, body);
        }

        public async Task SendEmailToManyAsync(IEnumerable<string> recipients, string subject, string body)
        {
            var smtpHost = _config["Email:SmtpHost"];
            var smtpPort = int.Parse(_config["Email:SmtpPort"]!);
            var smtpUser = _config["Email:Username"];
            var smtpPass = _config["Email:Password"];
            var fromAddress = _config["Email:From"];

            using var smtp = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(fromAddress),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            foreach (var recipient in recipients)
                mail.To.Add(recipient);

            await smtp.SendMailAsync(mail);
        }
    }
}

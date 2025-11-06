using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AuthService.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpHost = _config["Smtp:Host"];
            var smtpPort = int.Parse(_config["Smtp:Port"] ?? "587");
            var smtpUser = _config["Smtp:User"];
            var smtpPass = _config["Smtp:Pass"];
            var enableSsl = bool.Parse(_config["Smtp:EnableSsl"] ?? "true");

            Console.WriteLine("[DEBUG] Host: " + smtpHost);
            Console.WriteLine("[DEBUG] User: " + smtpUser);
            Console.WriteLine("[DEBUG] Pass: " + (smtpPass != null ? "********" : "NULL"));
            Console.WriteLine("[DEBUG] To: " + to);

            if (smtpHost == null || smtpUser == null || smtpPass == null)
                throw new Exception("❌ Faltan valores SMTP en appsettings.json");

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = enableSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(smtpUser, "NeuroByte Soporte"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await client.SendMailAsync(mail);
        }

    }
}

using System.Net;
using System.Net.Mail;

namespace FutbolYa.WebAPI.Helpers
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarAsync(string para, string asunto, string htmlBody)
        {
            var host = _config["Smtp:Host"];
            var port = int.Parse(_config["Smtp:Port"] ?? "587");
            var user = _config["Smtp:User"];
            var pass = _config["Smtp:Pass"];
            var enableSsl = bool.Parse(_config["Smtp:EnableSsl"] ?? "true");

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(user, pass)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(user, "FutbolYa"),
                Subject = asunto,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mail.To.Add(para);

            await client.SendMailAsync(mail);
        }
    }
}

using System.Net;
using System.Net.Mail;

namespace ProyectoWebG2.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);
    }

    public class EmailService : IEmailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;
        private readonly string _fromName;

        public EmailService(IConfiguration cfg)
        {
            _host = cfg["Smtp:Host"] ?? throw new InvalidOperationException("Falta Smtp:Host");
            _port = int.TryParse(cfg["Smtp:Port"], out var p) ? p : 587;
            _user = cfg["Smtp:User"] ?? throw new InvalidOperationException("Falta Smtp:User");
            _pass = cfg["Smtp:Pass"] ?? throw new InvalidOperationException("Falta Smtp:Pass");
            _fromName = cfg["Smtp:FromName"] ?? "No-Reply";
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            using var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_user, _pass),
                EnableSsl = true // Office365 usa STARTTLS en 587
            };

            var msg = new MailMessage
            {
                From = new MailAddress(_user, _fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            msg.To.Add(new MailAddress(to));

            await client.SendMailAsync(msg);
        }
    }
}

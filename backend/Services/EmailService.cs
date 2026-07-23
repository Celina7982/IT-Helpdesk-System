using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using IThelpdesk.Interfaces.Services;

namespace IThelpdesk.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string recipientEmail,
            string subject,
            string htmlBody)
        {
            var message = new MimeMessage();

            // Sender configuration
            string senderName = _configuration["Smtp:SenderName"] ?? "LBC IT Helpdesk";
            string senderEmail = _configuration["Smtp:SenderEmail"] ?? "Unreleasedmusic090@gmail.com";

            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(MailboxAddress.Parse(recipientEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = "Notification from IT Helpdesk System."
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // Connect to Gmail SMTP
                string host = _configuration["Smtp:Host"] ?? "smtp.gmail.com";
                int port = int.Parse(_configuration["Smtp:Port"] ?? "587");

                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);

                // Authenticate with App Password
                string username = _configuration["Smtp:Username"] ?? "Unreleasedmusic090@gmail.com";
                string password = _configuration["Smtp:Password"]?? "kgqm pqfq rejk tfzf"; // Gmail App Password

                await client.AuthenticateAsync(username, password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
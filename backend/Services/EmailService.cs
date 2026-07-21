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

            message.From.Add(
                new MailboxAddress(
                    _configuration["Smtp:SenderName"],
                    _configuration["Smtp:SenderEmail"]
                )
            );

            message.To.Add(
                MailboxAddress.Parse(recipientEmail)
            );

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = "This email was sent by the IT Helpdesk System."
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(
                    _configuration["Smtp:Host"],
                    int.Parse(_configuration["Smtp:Port"]!),
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    _configuration["Smtp:Username"],
                    _configuration["Smtp:Password"]
                );

                await client.SendAsync(message);

                await client.DisconnectAsync(true);
            }
        }
    }
}
namespace IThelpdesk.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string recipientEmail,
            string subject,
            string htmlBody);
    }
}
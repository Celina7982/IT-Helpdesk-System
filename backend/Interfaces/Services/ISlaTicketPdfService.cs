using System.Threading.Tasks;

namespace IThelpdesk.Interfaces.Services
{
    public interface ISlaTicketPdfService
    {
        Task<byte[]> GenerateSlaTicketPdfAsync(int slaTicketId);
    }
}
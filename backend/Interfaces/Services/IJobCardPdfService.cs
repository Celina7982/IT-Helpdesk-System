using System.Threading.Tasks;

namespace IThelpdesk.Interfaces.Services
{
    public interface IJobCardPdfService
    {
        Task<byte[]> GenerateJobCardPdfAsync(int jobCardId);
    }
}

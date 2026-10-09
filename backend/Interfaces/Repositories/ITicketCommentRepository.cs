using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface ITicketCommentRepository
    {
        Task<bool> TicketExistsAsync(int ticketId);

        Task<Ticket?> GetTicketByIdAsync(int ticketId);

        Task<List<TicketComment>> GetByTicketIdAsync(int ticketId);

        Task<TicketComment?> GetByIdAsync(
            int ticketId,
            int commentId);

        Task AddAsync(TicketComment comment);

        Task SaveChangesAsync();
    }
}

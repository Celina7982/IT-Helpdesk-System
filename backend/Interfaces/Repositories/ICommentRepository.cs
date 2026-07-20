using IThelpdesk.Models;

public interface ICommentRepository
{
    Task AddAsync(TicketComment comment);

    Task<IEnumerable<TicketComment>> GetCommentsByTicketIdAsync(int ticketId);

    Task SaveChangesAsync();
}
using IThelpdesk.Models;

public interface ICommentService
{
    Task AddCommentAsync(int ticketId, int userId, string comment);

    Task<IEnumerable<TicketComment>> GetCommentsAsync(int ticketId);
}
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;

    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task AddCommentAsync(int ticketId, int userId, string comment)
    {
        var ticketComment = new TicketComment
        {
            TicketId = ticketId,
            UserId = userId,
            Comment = comment
        };

        await _commentRepository.AddAsync(ticketComment);
        await _commentRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<TicketComment>> GetCommentsAsync(int ticketId)
    {
        return await _commentRepository.GetCommentsByTicketIdAsync(ticketId);
    }
}
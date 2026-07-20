using Microsoft.EntityFrameworkCore;
using IThelpdesk.Data;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Models;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;

    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TicketComment comment)
    {
        await _context.TicketComments.AddAsync(comment);
    }

    public async Task<IEnumerable<TicketComment>> GetCommentsByTicketIdAsync(int ticketId)
    {
        return await _context.TicketComments
            .Where(c => c.TicketId == ticketId)
            .Include(c => c.User)
            .OrderBy(c => c.CreatedDate)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
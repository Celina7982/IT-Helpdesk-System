using IThelpdesk.Data;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Models;
using Microsoft.EntityFrameworkCore;

namespace IThelpdesk.Repositories
{
    public class TicketCommentRepository : ITicketCommentRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketCommentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        //-------------------------------------------------------
        // Check Ticket Exists
        //-------------------------------------------------------

        public async Task<bool> TicketExistsAsync(int ticketId)
        {
            return await _context.Tickets
                .AnyAsync(t => t.TicketId == ticketId);
        }

        //-------------------------------------------------------
        // Get Ticket
        //-------------------------------------------------------

        public async Task<Ticket?> GetTicketByIdAsync(int ticketId)
        {
            return await _context.Tickets
                .FirstOrDefaultAsync(t =>
                    t.TicketId == ticketId);
        }

        //-------------------------------------------------------
        // Get Comments For Ticket
        //-------------------------------------------------------

        public async Task<List<TicketComment>> GetByTicketIdAsync(
            int ticketId)
        {
            return await _context.TicketComments
                .AsNoTracking()
                .Where(c => c.TicketId == ticketId)
                .OrderBy(c => c.CreatedDate)
                .ToListAsync();
        }

        //-------------------------------------------------------
        // Get Single Comment
        //-------------------------------------------------------

        public async Task<TicketComment?> GetByIdAsync(
            int ticketId,
            int commentId)
        {
            return await _context.TicketComments
                .FirstOrDefaultAsync(c =>
                    c.CommentId == commentId &&
                    c.TicketId == ticketId);
        }

        //-------------------------------------------------------
        // Add Comment
        //-------------------------------------------------------

        public async Task AddAsync(TicketComment comment)
        {
            await _context.TicketComments.AddAsync(comment);
        }

        //-------------------------------------------------------
        // Save Changes
        //-------------------------------------------------------

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using IThelpdesk.Data;

using IThelpdesk.Models;
using IThelpdesk.Interfaces.Repositories;

namespace IThelpdesk.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _context.Tickets.ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets.FindAsync(id);
        }


        public async Task<IEnumerable<Ticket>> GetAvailableTicketsAsync()
        {
            return await _context.Tickets
                .Where(t => t.AssignedToUserId == null)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetMyTicketsAsync(int technicianId)
        {
            return await _context.Tickets
                .Where(t => t.AssignedToUserId == technicianId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
        public async Task AddAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Ticket>> GetEscalatedTicketsAsync()
        {
            return await _context.Tickets
                .Where(t => t.IsEscalated)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Ticket>> GetMyTicketsByUserAsync(int userId)
        {
            return await _context.Tickets
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

    }
}
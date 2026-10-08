using IThelpdesk.Data;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Models;
using Microsoft.EntityFrameworkCore;

namespace IThelpdesk.Repositories
{
    public class SlaTicketRepository : ISlaTicketRepository
    {
        private readonly ApplicationDbContext _context;

        public SlaTicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        //--------------------------------------------------
        // Get All SLA Tickets
        //--------------------------------------------------

        public async Task<IEnumerable<SlaTicket>> GetAllAsync()
        {
            return await _context.SlaTickets
                .Include(s => s.Ticket)
                .Include(s => s.Technician)
                .OrderByDescending(s => s.DateCreated)
                .ToListAsync();
        }


        //--------------------------------------------------
        // Get SLA Ticket By Id
        //--------------------------------------------------

        public async Task<SlaTicket?> GetByIdAsync(int id)
        {
            return await _context.SlaTickets
                .Include(s => s.Ticket)
                .Include(s => s.Technician)
                .Include(s => s.CreatedByUser)
                .Include(s => s.EmailedByUser)
                .FirstOrDefaultAsync(s => s.SlaTicketId == id);
        }


        //--------------------------------------------------
        // Get SLA Ticket By Original Ticket Id
        //--------------------------------------------------

        public async Task<SlaTicket?> GetByTicketIdAsync(int ticketId)
        {
            return await _context.SlaTickets
                .Include(s => s.Ticket)
                .Include(s => s.Technician)
                .FirstOrDefaultAsync(s => s.TicketId == ticketId);
        }


        //--------------------------------------------------
        // Get Latest SLA Ticket
        //--------------------------------------------------

        public async Task<SlaTicket?> GetLatestSlaTicketAsync()
        {
            return await _context.SlaTickets
                .OrderByDescending(s => s.SlaTicketId)
                .FirstOrDefaultAsync();
        }


        //--------------------------------------------------
        // Add SLA Ticket
        //--------------------------------------------------

        public async Task AddAsync(SlaTicket slaTicket)
        {
            await _context.SlaTickets.AddAsync(slaTicket);
            await _context.SaveChangesAsync();
        }


        //--------------------------------------------------
        // Update SLA Ticket
        //--------------------------------------------------

        public async Task UpdateAsync(SlaTicket slaTicket)
        {
            _context.SlaTickets.Update(slaTicket);
            await _context.SaveChangesAsync();
        }


        //--------------------------------------------------
        // Delete SLA Ticket
        //--------------------------------------------------

        public async Task DeleteAsync(SlaTicket slaTicket)
        {
            _context.SlaTickets.Remove(slaTicket);
            await _context.SaveChangesAsync();
        }


        //--------------------------------------------------
        // Save Changes
        //--------------------------------------------------

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        //--------------------------------------------------
        // Check SLA Number
        //--------------------------------------------------

        public async Task<bool> SlaNumberExistsAsync(string slaNumber)
        {
            return await _context.SlaTickets
                .AnyAsync(s => s.SlaNumber == slaNumber);
        }
    }
}
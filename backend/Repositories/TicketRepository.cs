using Microsoft.EntityFrameworkCore;
using IThelpdesk.Data;
using IThelpdesk.DTOs.Ticket;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Models;

namespace IThelpdesk.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //-------------------------------------------------------
        // Get All Tickets (Admin)
        //-------------------------------------------------------

        public async Task<IEnumerable<TicketResponseDto>> GetAllAsync()
        {
            return await _context.Tickets

                .Include(t => t.AssignedToUser)

                .Where(t => !t.IsArchived)

                .OrderByDescending(t => t.CreatedDate)

                .Select(t => new TicketResponseDto
                {
                    TicketId = t.TicketId,

                    Subject = t.Subject,

                    Status = t.Status,

                    Priority = t.Priority,

                    CustomerName = t.CustomerName,

                    CompanyName = t.CompanyName ?? "",

                    CreatedDate = t.CreatedDate,

                    IsEscalated = t.IsEscalated,

                    AssignedTechnician =
                        t.AssignedToUser != null
                            ? t.AssignedToUser.FirstName + " " +
                              t.AssignedToUser.LastName
                            : "Not Assigned",

                    //---------------------------------------------------
                    // Job Card information
                    //---------------------------------------------------

                    HasJobCard = _context.JobCards
                        .Any(j => j.TicketId == t.TicketId),

                    JobCardId = _context.JobCards
                        .Where(j => j.TicketId == t.TicketId)
                        .Select(j => (int?)j.JobCardId)
                        .FirstOrDefault()
                })

                .ToListAsync();
        }

        //-------------------------------------------------------
        // Get Ticket By Id
        //-------------------------------------------------------

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets.FindAsync(id);
        }

        //-------------------------------------------------------
        // Get User By Id
        //-------------------------------------------------------

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        //-------------------------------------------------------
        // Available Tickets
        //-------------------------------------------------------

        public async Task<IEnumerable<Ticket>> GetAvailableTicketsAsync()
        {
            return await _context.Tickets

                .Where(t =>
                    t.AssignedToUserId == null &&
                    !t.IsArchived)

                .OrderByDescending(t => t.CreatedDate)

                .ToListAsync();
        }

        //-------------------------------------------------------
        // Technician / Admin My Tickets
        //-------------------------------------------------------

        public async Task<IEnumerable<TicketResponseDto>> GetMyTicketsAsync(
            int technicianId)
        {
            return await _context.Tickets

                .Include(t => t.AssignedToUser)

                .Where(t =>
                    t.AssignedToUserId == technicianId &&
                    !t.IsArchived)

                .OrderByDescending(t => t.CreatedDate)

                .Select(t => new TicketResponseDto
                {
                    TicketId = t.TicketId,

                    Subject = t.Subject,

                    Status = t.Status,

                    Priority = t.Priority,

                    CustomerName = t.CustomerName,

                    CompanyName = t.CompanyName ?? "",

                    CreatedDate = t.CreatedDate,

                    IsEscalated = t.IsEscalated,

                    AssignedTechnician =
                        t.AssignedToUser != null
                            ? t.AssignedToUser.FirstName + " " +
                              t.AssignedToUser.LastName
                            : "Not Assigned",

                    //---------------------------------------------------
                    // Job Card information
                    //---------------------------------------------------

                    HasJobCard = _context.JobCards
                        .Any(j => j.TicketId == t.TicketId),

                    JobCardId = _context.JobCards
                        .Where(j => j.TicketId == t.TicketId)
                        .Select(j => (int?)j.JobCardId)
                        .FirstOrDefault()
                })

                .ToListAsync();
        }

        //-------------------------------------------------------
        // Escalated Tickets
        //-------------------------------------------------------

        public async Task<IEnumerable<Ticket>> GetEscalatedTicketsAsync()
        {
            return await _context.Tickets

                .Where(t =>
                    t.IsEscalated &&
                    t.Status == "Escalated" &&
                    !t.IsArchived)

                .OrderByDescending(t => t.CreatedDate)

                .ToListAsync();
        }

        //-------------------------------------------------------
        // Client Tickets
        //-------------------------------------------------------

        public async Task<IEnumerable<Ticket>> GetMyTicketsByUserAsync(int userId)
        {
            return await _context.Tickets

                .Where(t =>
                    t.UserId == userId &&
                    !t.IsArchived)

                .OrderByDescending(t => t.CreatedDate)

                .ToListAsync();
        }

        //-------------------------------------------------------
        // Ticket Details
        //-------------------------------------------------------

        public async Task<TicketDetailsDto?> GetTicketDetailsAsync(int id)
        {
            return await _context.Tickets

                .Include(t => t.AssignedToUser)

                .Where(t => t.TicketId == id)

                .Select(t => new TicketDetailsDto
                {
                    TicketId = t.TicketId,

                    Subject = t.Subject,

                    Description = t.Description,

                    CustomerName = t.CustomerName,

                    CompanyName = t.CompanyName,

                    Category = t.Category,

                    Priority = t.Priority,

                    Status = t.Status,

                    CreatedDate = t.CreatedDate,

                    AssignedTechnician =
                        t.AssignedToUser != null
                            ? t.AssignedToUser.FirstName + " " +
                              t.AssignedToUser.LastName
                            : "Not Assigned",

                    IsEscalated = t.IsEscalated,

                    EscalationReason = t.EscalationReason
                })

                .FirstOrDefaultAsync();
        }

        //-------------------------------------------------------
        // CRUD
        //-------------------------------------------------------

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

        //-------------------------------------------------------
        // Archive Ticket
        //-------------------------------------------------------

        public async Task ArchiveAsync(Ticket ticket)
        {
            ticket.IsArchived = true;

            ticket.ArchivedDate = DateTime.UtcNow;

            _context.Tickets.Update(ticket);

            await Task.CompletedTask;
        }

        //-------------------------------------------------------
        // Save Changes
        //-------------------------------------------------------

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        //-------------------------------------------------------
        // Archived Tickets
        //-------------------------------------------------------

        public async Task<IEnumerable<TicketResponseDto>> GetArchivedTicketsAsync()
        {
            return await _context.Tickets

                .Include(t => t.AssignedToUser)

                .Where(t => t.IsArchived)

                .OrderByDescending(t => t.ArchivedDate)

                .Select(t => new TicketResponseDto
                {
                    TicketId = t.TicketId,

                    Subject = t.Subject,

                    Status = t.Status,

                    Priority = t.Priority,

                    CustomerName = t.CustomerName,

                    CompanyName = t.CompanyName ?? "",

                    CreatedDate = t.CreatedDate,

                    IsEscalated = t.IsEscalated,

                    AssignedTechnician =
                        t.AssignedToUser != null
                            ? t.AssignedToUser.FirstName + " " +
                              t.AssignedToUser.LastName
                            : "Not Assigned",

                    HasJobCard = _context.JobCards
                        .Any(j => j.TicketId == t.TicketId),

                    JobCardId = _context.JobCards
                        .Where(j => j.TicketId == t.TicketId)
                        .Select(j => (int?)j.JobCardId)
                        .FirstOrDefault()
                })

                .ToListAsync();
        }
    }
}

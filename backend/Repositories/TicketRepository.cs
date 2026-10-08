using IThelpdesk.Data;
using IThelpdesk.DTOs.Ticket;
using IThelpdesk.Entities;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<TicketResponseDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Tickets
                .Include(t => t.AssignedToUser)
                .Where(t => !t.IsArchived)
                .OrderByDescending(t => t.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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


        // ----------------------------------------------------
        // Multiple Ticket Assignments
        // ----------------------------------------------------

        public async Task<List<TicketAssignment>> GetTicketAssignmentsAsync(
            int ticketId)
        {
            return await _context.TicketAssignments
                .AsNoTracking()
                .Include(a => a.User)
                .Include(a => a.AssignedByUser)
                .Where(a => a.TicketId == ticketId)
                .OrderBy(a => a.AssignedDate)
                .ToListAsync();
        }


        public async Task<TicketAssignment?> GetTicketAssignmentAsync(
            int ticketId,
            int userId)
        {
            return await _context.TicketAssignments
                .FirstOrDefaultAsync(a =>
                    a.TicketId == ticketId &&
                    a.UserId == userId);
        }


        public async Task AddTicketAssignmentAsync(
            TicketAssignment assignment)
        {
            await _context.TicketAssignments.AddAsync(assignment);
        }


        public Task RemoveTicketAssignmentAsync(
            TicketAssignment assignment)
        {
            _context.TicketAssignments.Remove(assignment);

            return Task.CompletedTask;
        }


        public async Task<bool> HasTicketAssignmentAsync(
            int ticketId,
            int userId)
        {
            return await _context.TicketAssignments
                .AnyAsync(a =>
                    a.TicketId == ticketId &&
                    a.UserId == userId);
        }


        //-------------------------------------------------------
        // Available Tickets
        // Only tickets with NO assigned users
        //-------------------------------------------------------

        public async Task<IEnumerable<Ticket>> GetAvailableTicketsAsync()
        {
            return await _context.Tickets
                .AsNoTracking()
                .Where(t =>

                    // No primary assignee
                    t.AssignedToUserId == null &&

                    // No additional assignees
                    !t.Assignments.Any() &&

                    // Ticket must be Open
                    t.Status == "Open" &&

                    // Exclude archived tickets
                    !t.IsArchived &&

                    // Exclude escalated tickets
                    !t.IsEscalated
                )
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }


        //-------------------------------------------------------
        // Technician / Admin My Tickets
        //-------------------------------------------------------

        public async Task<IEnumerable<TicketResponseDto>> GetMyTicketsAsync(
      int technicianId, int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Tickets
                .Include(t => t.AssignedToUser)

            .Where(t =>
                !t.IsArchived &&
                t.Assignments.Any(a =>
                    a.UserId == technicianId))

                .OrderByDescending(t => t.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
    .FirstOrDefault(),


                    //---------------------------------------------------
                    // SLA Report information
                    //---------------------------------------------------

                    HasSlaTicket = _context.SlaTickets
    .Any(s => s.TicketId == t.TicketId),

                    SlaTicketId = _context.SlaTickets
    .Where(s => s.TicketId == t.TicketId)
    .Select(s => (int?)s.SlaTicketId)
    .FirstOrDefault()
                })
                .ToListAsync();
        }

        //-------------------------------------------------------
        // Escalated Tickets
        //-------------------------------------------------------

        public async Task<IEnumerable<Ticket>> GetEscalatedTicketsAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Tickets
                .Where(t =>
                    t.IsEscalated &&
                    t.Status == "Escalated" &&
                    !t.IsArchived)
                .OrderByDescending(t => t.CreatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        //-------------------------------------------------------
        // Archived Tickets
        //-------------------------------------------------------

        public async Task<IEnumerable<TicketResponseDto>> GetArchivedTicketsAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Tickets
                .Include(t => t.AssignedToUser)
                .Where(t => t.IsArchived)
                .OrderByDescending(t => t.ArchivedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
                            ? t.AssignedToUser.FirstName + " " + t.AssignedToUser.LastName
                            : "Not Assigned",

                    HasJobCard = _context.JobCards.Any(j => j.TicketId == t.TicketId),
                    JobCardId = _context.JobCards
                        .Where(j => j.TicketId == t.TicketId)
                        .Select(j => (int?)j.JobCardId)
                        .FirstOrDefault()
                })
                .ToListAsync();
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

       
    }
}
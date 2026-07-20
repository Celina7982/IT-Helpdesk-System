using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();

        Task<Ticket?> GetTicketByIdAsync(int id);

        Task<IEnumerable<Ticket>> GetAvailableTicketsAsync();

        Task<IEnumerable<Ticket>> GetMyTicketsAsync(int technicianId);

        Task CreateTicketAsync(Ticket ticket);

        Task UpdateTicketAsync(Ticket ticket);

        Task DeleteTicketAsync(int id);

        // Admin assigns an escalated ticket
        Task AssignTicketAsync(int ticketId, int assignedToUserId);

        // Technician claims an open ticket
        Task ClaimTicketAsync(int ticketId, int technicianId);

        // Technician escalates a ticket
        Task EscalateTicketAsync(int ticketId, string escalationReason);

        // Admin or Technician resolves a ticket
        Task ResolveTicketAsync(int ticketId);

        
        Task<IEnumerable<Ticket>> GetEscalatedTicketsAsync();

        Task<IEnumerable<Ticket>> GetMyTicketsByUserAsync(int userId);
    }
}
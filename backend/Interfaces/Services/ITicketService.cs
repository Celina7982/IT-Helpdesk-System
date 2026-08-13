using IThelpdesk.DTOs.Ticket;
using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Services
{
    public interface ITicketService
    {
        //-------------------------------------------------------
        // Ticket Lists
        //-------------------------------------------------------

        Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync();

        Task<IEnumerable<Ticket>> GetAvailableTicketsAsync();

        Task<IEnumerable<TicketResponseDto>> GetMyTicketsAsync(int technicianId);

        Task<IEnumerable<Ticket>> GetEscalatedTicketsAsync();

        Task<IEnumerable<Ticket>> GetMyTicketsByUserAsync(int userId);

        Task<IEnumerable<TicketResponseDto>> GetArchivedTicketsAsync();


        //-------------------------------------------------------
        // Single Ticket
        //-------------------------------------------------------

        Task<Ticket?> GetTicketByIdAsync(int id);

        Task<TicketDetailsDto?> GetTicketDetailsAsync(int id);


        //-------------------------------------------------------
        // CRUD
        //-------------------------------------------------------

        Task CreateTicketAsync(Ticket ticket);

        Task UpdateTicketAsync(Ticket ticket);

        Task DeleteTicketAsync(int id);


        //-------------------------------------------------------
        // Ticket Actions
        //-------------------------------------------------------

        Task AssignTicketAsync(int ticketId, int assignedToUserId);

        Task ClaimTicketAsync(int ticketId, int technicianId);

        Task EscalateTicketAsync(
            int ticketId,
            string escalationReason);

        Task ResolveTicketAsync(
            int id,
            int resolvedByUserId);

        //-------------------------------------------------------
        // Archive
        //-------------------------------------------------------

        Task ArchiveTicketAsync(int ticketId);
    }
}
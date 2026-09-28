using IThelpdesk.DTOs.Ticket;
using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Services
{
    public interface ITicketService
    {
        //-------------------------------------------------------
        // Ticket Lists
        //-------------------------------------------------------
        
        //updated with serverside pagination
        Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync(int pageNumber = 1, int pageSize = 10);

        Task<IEnumerable<Ticket>> GetAvailableTicketsAsync();

        //includes serverside pagination
        Task<IEnumerable<TicketResponseDto>> GetMyTicketsAsync(int technicianId, int pageNumber = 1, int pageSize = 10);
      

        Task<IEnumerable<Ticket>> GetMyTicketsByUserAsync(int userId);

        Task<IEnumerable<Ticket>> GetEscalatedTicketsAsync(int pageNumber = 1, int pageSize = 10);

        Task<IEnumerable<TicketResponseDto>> GetArchivedTicketsAsync(int pageNumber = 1, int pageSize = 10);


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
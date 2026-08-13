using IThelpdesk.Models;
using IThelpdesk.DTOs.Ticket;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        //-------------------------------------------------------
        // Ticket Lists
        //-------------------------------------------------------

        Task<IEnumerable<TicketResponseDto>> GetAllAsync();

        Task<IEnumerable<Ticket>> GetAvailableTicketsAsync();

        Task<IEnumerable<TicketResponseDto>> GetMyTicketsAsync(int technicianId);

        Task<IEnumerable<Ticket>> GetEscalatedTicketsAsync();

        Task<IEnumerable<Ticket>> GetMyTicketsByUserAsync(int userId);

        Task<IEnumerable<TicketResponseDto>> GetArchivedTicketsAsync();


        //-------------------------------------------------------
        // Single Ticket
        //-------------------------------------------------------

        Task<Ticket?> GetByIdAsync(int id);

        Task<TicketDetailsDto?> GetTicketDetailsAsync(int id);

        Task<User?> GetUserByIdAsync(int id);


        //-------------------------------------------------------
        // CRUD
        //-------------------------------------------------------

        Task AddAsync(Ticket ticket);

        Task UpdateAsync(Ticket ticket);

        Task DeleteAsync(Ticket ticket);


        //-------------------------------------------------------
        // Archive
        //-------------------------------------------------------

        Task ArchiveAsync(Ticket ticket);


        //-------------------------------------------------------
        // Save
        //-------------------------------------------------------

        Task SaveChangesAsync();
    }
}
using IThelpdesk.Models;
using IThelpdesk.DTOs.Ticket;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        //-------------------------------------------------------
        // Ticket Lists
        //-------------------------------------------------------

        Task<IEnumerable<TicketResponseDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10);

        Task<IEnumerable<Ticket>> GetAvailableTicketsAsync();

        Task<IEnumerable<TicketResponseDto>> GetMyTicketsAsync(int technicianId, int pageNumber = 1, int pageSize = 10);

       


        Task<IEnumerable<Ticket>> GetMyTicketsByUserAsync(int userId);

        Task<IEnumerable<Ticket>> GetEscalatedTicketsAsync(int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<TicketResponseDto>> GetArchivedTicketsAsync(int pageNumber = 1, int pageSize = 10);


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
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

        Task<IEnumerable<TechnicianTicketDto>> GetMyTicketsAsync(int technicianId);

        Task<IEnumerable<Ticket>> GetEscalatedTicketsAsync();

        Task<IEnumerable<Ticket>> GetMyTicketsByUserAsync(int userId);

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

        Task SaveChangesAsync();
    }
}
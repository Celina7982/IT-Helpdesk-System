using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllAsync();

        Task<Ticket?> GetByIdAsync(int id);

        Task<IEnumerable<Ticket>> GetAvailableTicketsAsync();

        Task<IEnumerable<Ticket>> GetMyTicketsAsync(int technicianId);

        Task<IEnumerable<Ticket>> GetEscalatedTicketsAsync();

        Task AddAsync(Ticket ticket);

        Task<IEnumerable<Ticket>> GetMyTicketsByUserAsync(int userId);

        Task UpdateAsync(Ticket ticket);

        Task DeleteAsync(Ticket ticket);

        Task SaveChangesAsync();
    }
}
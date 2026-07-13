using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllAsync();

        Task<Ticket?> GetByIdAsync(int id);

        Task AddAsync(Ticket ticket);

        Task UpdateAsync(Ticket ticket);

        Task DeleteAsync(Ticket ticket);

        Task SaveChangesAsync();
    }
}
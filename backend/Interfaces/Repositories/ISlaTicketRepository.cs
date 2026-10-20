using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface ISlaTicketRepository
    {
        //--------------------------------------------------
        // Get
        //--------------------------------------------------

        Task<IEnumerable<SlaTicket>> GetAllAsync();

        Task<SlaTicket?> GetByIdAsync(int id);

        Task<SlaTicket?> GetByTicketIdAsync(int ticketId);

        Task<SlaTicket?> GetLatestSlaTicketAsync();


        //--------------------------------------------------
        // CRUD
        //--------------------------------------------------

        Task AddAsync(SlaTicket slaTicket);

        Task UpdateAsync(SlaTicket slaTicket);

        Task DeleteAsync(SlaTicket slaTicket);

        Task SaveChangesAsync();


        //--------------------------------------------------
        // Validation
        //--------------------------------------------------

        Task<bool> SlaNumberExistsAsync(string slaNumber);
    }
}
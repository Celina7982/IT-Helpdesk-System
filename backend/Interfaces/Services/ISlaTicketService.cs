using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Services
{
    public interface ISlaTicketService
    {
        //--------------------------------------------------
        // Create
        //--------------------------------------------------

        Task<SlaTicket> CreateFromTicketAsync(
            int ticketId,
            int performedByUserId);


        //--------------------------------------------------
        // Get
        //--------------------------------------------------

        Task<IEnumerable<SlaTicket>> GetAllAsync();

        Task<SlaTicket?> GetByIdAsync(int id);

        Task<SlaTicket?> GetByTicketIdAsync(int ticketId);


        //--------------------------------------------------
        // Update
        //--------------------------------------------------

        Task<SlaTicket> UpdateAsync(
            int id,
            string workPerformed,
            string resolutionNotes,
            string status,
            int performedByUserId);


        //--------------------------------------------------
        // Delete
        //--------------------------------------------------

        Task DeleteAsync(
            int id,
            int performedByUserId);
    }
}
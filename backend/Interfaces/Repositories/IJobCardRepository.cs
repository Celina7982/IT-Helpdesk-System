using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface IJobCardRepository
    {
        //--------------------------------------------------
        // Job Cards
        //--------------------------------------------------

        Task<IEnumerable<JobCard>> GetAllAsync();

        // Returns lightweight data for the Job Card List page.
        Task<IEnumerable<JobCardListDto>> GetJobCardListAsync();

        Task<JobCard?> GetByIdAsync(int id);

        Task<JobCardDetailsDto?> GetDetailsAsync(int id);

        Task AddAsync(JobCard jobCard);

        Task UpdateAsync(JobCard jobCard);

        Task DeleteAsync(JobCard jobCard);

        Task SaveChangesAsync();

        //--------------------------------------------------
        // Helpers
        //--------------------------------------------------

        Task<bool> JobNumberExistsAsync(string jobNumber);

        Task<JobCard?> GetByTicketIdAsync(int ticketId);

        Task<JobCard?> GetLatestJobCardAsync();

        //--------------------------------------------------
        // Labour
        //--------------------------------------------------

        Task AddLabourEntryAsync(JobCardLabour labour);

        Task<List<JobCardLabour>> GetLabourEntriesAsync(int jobCardId);



        //=============================
        //Job card list
        //=============================

        // Returns all Job Cards assigned to a technician.
        Task<IEnumerable<JobCardListDto>> GetJobCardListByTechnicianAsync(int technicianId);
    }
}
using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Models;
using IThelpdesk.DTOs.Common;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface IJobCardRepository
    {
        //--------------------------------------------------
        // Job Cards
        //--------------------------------------------------

        /// <summary>
        /// Returns all Job Cards.
        /// </summary>
        Task<IEnumerable<JobCard>> GetAllAsync();

        /// <summary>
        /// Returns Job Cards for the Job Card List page.
        /// Supports filtering, searching and sorting.
        /// </summary>
        Task<PagedResultDto<JobCardListDto>> GetJobCardListAsync(
            int? technicianId,
            string? status,
            int? assignedTo,
            string? search,
            string? sortBy,
            string? sortDirection,
            int pageNumber,
            int pageSize);

        /// <summary>
        /// Returns a Job Card by Id.
        /// </summary>
        Task<JobCard?> GetByIdAsync(int id);

        /// <summary>
        /// Returns the Job Card linked to a Ticket.
        /// </summary>
        Task<JobCard?> GetByTicketIdAsync(int ticketId);

        /// <summary>
        /// Returns the latest Job Card.
        /// Used for Job Number generation.
        /// </summary>
        Task<JobCard?> GetLatestJobCardAsync();

        /// <summary>
        /// Returns complete Job Card details.
        /// </summary>
        Task<JobCardDetailsDto?> GetDetailsAsync(int id);

        //--------------------------------------------------
        // CRUD
        //--------------------------------------------------

        Task AddAsync(JobCard jobCard);

        Task UpdateAsync(JobCard jobCard);

        Task DeleteAsync(JobCard jobCard);

        Task SaveChangesAsync();

        //--------------------------------------------------
        // Validation
        //--------------------------------------------------

        Task<bool> JobNumberExistsAsync(string jobNumber);

        //--------------------------------------------------
        // Labour
        //--------------------------------------------------

        Task AddLabourEntryAsync(JobCardLabour labour);

        Task<List<JobCardLabour>> GetLabourEntriesAsync(int jobCardId);

        //--------------------------------------------------
        // Parts
        //--------------------------------------------------

        Task<List<JobCardPart>> GetPartsAsync(int jobCardId);
        Task<JobCardPart?> GetPartByIdAsync(int partId);
        Task AddPartAsync(JobCardPart part);
        Task UpdatePartAsync(JobCardPart part);
        Task DeletePartAsync(JobCardPart part);

    }
}
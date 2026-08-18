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

        // Returns a single labour entry by id including related JobCard and Technician
        Task<JobCardLabour?> GetLabourEntryByIdAsync(int labourId);

        // Marks an existing labour entry as updated in the DbContext (does not SaveChanges)
        Task UpdateLabourEntryAsync(JobCardLabour labour);

        // Removes a labour entry from the DbContext (does not SaveChanges)
        Task DeleteLabourEntryAsync(JobCardLabour labour);

        //--------------------------------------------------
        // Parts
        //--------------------------------------------------

        /// <summary>
        /// Adds a Part to a Job Card.
        /// </summary>
        Task AddPartAsync(JobCardPart part);

        /// <summary>
        /// Returns all Parts used on a Job Card.
        /// </summary>
        Task<List<JobCardPart>> GetPartsAsync(int jobCardId);

        /// <summary>
        /// Returns a Part by Id.
        /// </summary>
        Task<JobCardPart?> GetPartByIdAsync(int partId);


        Task UpdatePartAsync(JobCardPart part);
        /// <summary>
        /// Deletes a Part from a Job Card.
        /// </summary>
        Task DeletePartAsync(JobCardPart part);

    }
}
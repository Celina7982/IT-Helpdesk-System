using IThelpdesk.DTOs.Common;
using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Services
{
    public interface IJobCardService
    {
        //--------------------------------------------------
        // Job Cards
        //--------------------------------------------------

        /// <summary>
        /// Returns all Job Cards.
        /// </summary>
        Task<IEnumerable<JobCard>> GetAllAsync();

        /// <summary>
        /// Returns the Job Card List.
        /// Supports role filtering, searching and sorting.
        /// </summary>
        Task<PagedResultDto<JobCardListDto>> GetJobCardListAsync(
         int userId,
         string role,
         bool mine,
         string? status,
         int? assignedTo,
         string? search,
         string? sortBy,
         string? sortDirection,
         int pageNumber,
         int pageSize);

        /// <summary>
        /// Returns a Job Card.
        /// </summary>
        Task<JobCard?> GetByIdAsync(int id);

        /// <summary>
        /// Returns Job Card Details.
        /// </summary>
        Task<JobCardDetailsDto?> GetDetailsAsync(int id);

        /// <summary>
        /// Creates a Job Card from a Ticket.
        /// </summary>
        Task<JobCard> CreateFromTicketAsync(
         int ticketId,
         int currentUserId);

        /// <summary>
        /// Updates a Job Card.
        /// </summary>
        Task UpdateAsync(JobCard jobCard);

        /// <summary>
        /// Updates editable Job Card fields.
        /// </summary>
        Task UpdateJobCardAsync(
    int id,
    UpdateJobCardDto dto,
    int currentUserId);

        /// <summary>
        /// Completes a Job Card.
        /// </summary>
        Task CompleteJobCardAsync(int id);

        /// <summary>
        /// Deletes a Job Card.
        /// </summary>
        Task DeleteAsync(
        int id,
        int currentUserId);

        //--------------------------------------------------
        // Labour
        //--------------------------------------------------

        Task AddLabourEntryAsync(
            int jobCardId,
            AddLabourEntryDto dto,
            int currentUserId);

        Task<List<JobCardLabour>> GetLabourEntriesAsync(
            int jobCardId);

        //--------------------------------------------------
        // Parts
        //--------------------------------------------------

        /// <summary>
        /// Adds a Part to a Job Card.
        /// </summary>
        Task AddPartAsync(
            int jobCardId,
            AddPartDto dto,
            int currentUserId);

        /// <summary>
        /// Returns all Parts used on a Job Card.
        /// </summary>
        Task<List<JobCardPartDto>> GetPartsAsync(
            int jobCardId);

        /// <summary>
        /// Deletes a Part from a Job Card.
        /// </summary>
        Task DeletePartAsync(
        int partId,
        int currentUserId);

    }


}
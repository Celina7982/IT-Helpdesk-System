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
        /// Legacy/general method used where filtering
        /// is not required.
        /// </summary>
        Task<IEnumerable<JobCard>> GetAllAsync();

        /// <summary>
        /// Returns the Job Card List based on the
        /// logged-in user's permissions.
        ///
        /// Admin
        ///     Returns all Job Cards.
        ///
        /// Technician
        ///     Returns only Job Cards assigned
        ///     to the logged-in technician.
        /// </summary>
        Task<IEnumerable<JobCardListDto>> GetJobCardListAsync(
            int userId,
            string role);

        /// <summary>
        /// Returns a Job Card entity by its ID.
        /// </summary>
        Task<JobCard?> GetByIdAsync(int id);

        /// <summary>
        /// Returns the complete Job Card details used by
        /// the Job Card Details page.
        /// </summary>
        Task<JobCardDetailsDto?> GetDetailsAsync(int id);

        /// <summary>
        /// Creates a new Job Card from a resolved Ticket.
        /// </summary>
        Task<JobCard> CreateFromTicketAsync(int ticketId);

        /// <summary>
        /// Updates a Job Card entity.
        /// </summary>
        Task UpdateAsync(JobCard jobCard);

        /// <summary>
        /// Updates editable Job Card information.
        /// </summary>
        Task UpdateJobCardAsync(
            int id,
            UpdateJobCardDto dto);

        //--------------------------------------------------
        // Complete Job Card
        //--------------------------------------------------

        /// <summary>
        /// Marks a Job Card as completed.
        ///
        /// This method:
        /// • Sets the Status to "Completed"
        /// • Sets the DateCompleted
        /// • Saves the changes
        ///
        /// Business rules relating to completion belong
        /// here rather than in the frontend.
        /// </summary>
        Task CompleteJobCardAsync(int id);

        /// <summary>
        /// Deletes a Job Card.
        /// </summary>
        Task DeleteAsync(int id);

        //--------------------------------------------------
        // Labour
        //--------------------------------------------------

        /// <summary>
        /// Adds a Labour Entry to a Job Card.
        /// </summary>
        Task AddLabourEntryAsync(
            int jobCardId,
            AddLabourEntryDto dto);

        /// <summary>
        /// Returns all Labour Entries for a Job Card.
        /// </summary>
        Task<List<JobCardLabour>> GetLabourEntriesAsync(int jobCardId);
    }
}
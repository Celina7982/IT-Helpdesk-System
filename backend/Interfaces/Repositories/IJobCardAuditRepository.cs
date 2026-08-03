using IThelpdesk.Models;
using IThelpdesk.DTOs.JobCard;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface IJobCardAuditRepository
    {
        //--------------------------------------------------
        // Create Audit Entry
        //--------------------------------------------------

        /// <summary>
        /// Adds a new audit entry.
        /// </summary>
        Task AddAsync(JobCardAudit audit);

        //--------------------------------------------------
        // Get Audit History
        //--------------------------------------------------

        /// <summary>
        /// Returns the complete audit history
        /// for a Job Card.
        /// </summary>
        Task<List<JobCardAudit>> GetByJobCardIdAsync(int jobCardId);
        Task<List<JobCardAuditDto>> GetAuditHistoryDtoAsync(int jobCardId);

        //--------------------------------------------------
        // Save Changes
        //--------------------------------------------------

        Task SaveChangesAsync();
    }
}

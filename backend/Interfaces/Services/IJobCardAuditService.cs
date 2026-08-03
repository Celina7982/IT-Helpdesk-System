using IThelpdesk.Enums;
using IThelpdesk.Models;
using IThelpdesk.DTOs.JobCard;

namespace IThelpdesk.Interfaces.Services
{
    public interface IJobCardAuditService
    {
        //--------------------------------------------------
        // Create Audit Entry
        //--------------------------------------------------

        /// <summary>
        /// Creates a new Job Card audit record.
        /// </summary>
        Task LogAsync(
        int jobCardId,
        int userId,
        JobCardAuditAction action,
        string description,
        string? oldValue = null,
        string? newValue = null);

        //--------------------------------------------------
        // Get Audit History
        //--------------------------------------------------

        /// <summary>
        /// Returns the complete audit history
        /// for a Job Card.
        /// </summary>
        Task<List<JobCardAudit>> GetAuditHistoryAsync(
            int jobCardId);

        Task<List<JobCardAuditDto>> GetAuditHistoryDtoAsync(int jobCardId);
    }
}
using IThelpdesk.Enums;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;
using IThelpdesk.DTOs.JobCard;

namespace IThelpdesk.Services
{
    public class JobCardAuditService : IJobCardAuditService
    {
        private readonly IJobCardAuditRepository _auditRepository;

        public JobCardAuditService(
            IJobCardAuditRepository auditRepository)
        {
            _auditRepository = auditRepository;
        }

        //--------------------------------------------------
        // Create Audit Entry
        //--------------------------------------------------

        public async Task LogAsync(
            int jobCardId,
            int userId,
            JobCardAuditAction action,
            string description,
            string? oldValue = null,
            string? newValue = null)
        {
            var audit = new JobCardAudit
            {
                JobCardId = jobCardId,
                UserId = userId,
                Action = action,
                Description = description,
                OldValue = oldValue,
                NewValue = newValue,
                DateCreated = DateTime.UtcNow
            };

            await _auditRepository.AddAsync(audit);

            await _auditRepository.SaveChangesAsync();
        }

        //--------------------------------------------------
        // Get Audit History
        //--------------------------------------------------

        public async Task<List<JobCardAudit>> GetAuditHistoryAsync(
            int jobCardId)
        {
            return await _auditRepository.GetByJobCardIdAsync(jobCardId);
        }

        //--------------------------------------------------
        // Get Audit History DTO
        //--------------------------------------------------

        public async Task<List<JobCardAuditDto>> GetAuditHistoryDtoAsync(
            int jobCardId)
        {
            return await _auditRepository.GetAuditHistoryDtoAsync(jobCardId);
        }
    }
}
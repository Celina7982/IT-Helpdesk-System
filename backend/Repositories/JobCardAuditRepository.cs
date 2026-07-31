using IThelpdesk.Data;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Models;
using Microsoft.EntityFrameworkCore;
using IThelpdesk.DTOs.JobCard;

namespace IThelpdesk.Repositories
{
    public class JobCardAuditRepository : IJobCardAuditRepository
    {
        private readonly ApplicationDbContext _context;

        public JobCardAuditRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        //--------------------------------------------------
        // Add Audit Entry
        //--------------------------------------------------

        public async Task AddAsync(JobCardAudit audit)
        {
            await _context.JobCardAudits.AddAsync(audit);
        }

        //--------------------------------------------------
        // Get Audit History
        //--------------------------------------------------

        public async Task<List<JobCardAudit>> GetByJobCardIdAsync(
            int jobCardId)
        {
            return await _context.JobCardAudits

                .Include(a => a.User)

                .Where(a => a.JobCardId == jobCardId)

                .OrderByDescending(a => a.DateCreated)

                .ToListAsync();
        }

        //--------------------------------------------------
        // Save Changes
        //--------------------------------------------------

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<JobCardAuditDto>> GetAuditHistoryDtoAsync(int jobCardId)
        {
            return await _context.JobCardAudits
                .Include(a => a.User)
                .Where(a => a.JobCardId == jobCardId)
                .OrderByDescending(a => a.DateCreated)
                .Select(a => new JobCardAuditDto
                {
                    AuditId = a.AuditId,
                    Action = a.Action.ToString(),
                    Description = a.Description,
                    OldValue = a.OldValue,
                    NewValue = a.NewValue,
                    PerformedBy = a.User != null
                        ? a.User.FirstName + " " + a.User.LastName
                        : "System",
                    DateCreated = a.DateCreated
                })
                .ToListAsync();
        }
    }
}

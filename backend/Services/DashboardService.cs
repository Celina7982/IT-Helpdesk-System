using Microsoft.EntityFrameworkCore;
using IThelpdesk.Data;
using IThelpdesk.DTOs.Dashboard;
using IThelpdesk.Interfaces.Services;

namespace IThelpdesk.Services
{
    /// <summary>
    /// Provides dashboard statistics for the Admin Dashboard.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor used for Dependency Injection.
        /// </summary>
        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns statistics displayed on the Admin Dashboard.
        /// </summary>
        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
        {
            var statistics = new DashboardStatisticsDto
            {
                TotalTickets = await _context.Tickets.CountAsync(),

                OpenTickets = await _context.Tickets
                    .CountAsync(t => t.Status == "Open"),

                InProgressTickets = await _context.Tickets
                    .CountAsync(t => t.Status == "In Progress"),

                ResolvedTickets = await _context.Tickets
                    .CountAsync(t => t.Status == "Resolved")
            };

            return statistics;

        }

        /// <summary>
        /// Returns the most recent tickets for the dashboard.
        /// </summary>
        public async Task<List<RecentTicketDto>> GetRecentTicketsAsync(int count = 5)
        {
            return await _context.Tickets

                .OrderByDescending(t => t.CreatedDate)

                .Take(count)

                .Select(t => new RecentTicketDto
                {
                    TicketId = t.TicketId,
                    Subject = t.Subject,
                    Status = t.Status,
                    Priority = t.Priority,
                    CreatedDate = t.CreatedDate
                })

                .ToListAsync();
        }
    }
}
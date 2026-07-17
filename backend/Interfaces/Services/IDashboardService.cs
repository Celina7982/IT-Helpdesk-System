using IThelpdesk.DTOs.Dashboard;

namespace IThelpdesk.Interfaces.Services
{
    /// <summary>
    /// Provides dashboard statistics for the application.
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Returns the statistics displayed on the Admin Dashboard.
        /// </summary>
        /// Any class that implements this interface must provide a method that returns dashboard statistics.
        Task<DashboardStatisticsDto> GetDashboardStatisticsAsync();

        Task<List<RecentTicketDto>> GetRecentTicketsAsync(int count = 5);
    }
}
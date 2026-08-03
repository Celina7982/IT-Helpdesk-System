using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IThelpdesk.Interfaces.Services;

namespace IThelpdesk.Controllers
{
    /// <summary>
    /// Provides statistics for the Admin Dashboard.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Returns dashboard statistics.
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var statistics = await _dashboardService.GetDashboardStatisticsAsync();

            return Ok(statistics);
        }

        /// <summary>
        /// Returns the most recent tickets.
        /// </summary>
        [HttpGet("recent-tickets")]
        public async Task<IActionResult> GetRecentTickets()
        {
            var tickets = await _dashboardService.GetRecentTicketsAsync();

            return Ok(tickets);
        }
    }
}
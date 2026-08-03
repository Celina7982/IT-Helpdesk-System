namespace IThelpdesk.DTOs.Dashboard
{
    /// <summary>
    /// Contains the statistics displayed on the Admin Dashboard.
    /// </summary>
    public class DashboardStatisticsDto
    {
        public int TotalTickets { get; set; }

        public int OpenTickets { get; set; }

        public int InProgressTickets { get; set; }

        public int ResolvedTickets { get; set; }
    }
}

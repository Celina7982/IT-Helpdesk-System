namespace IThelpdesk.DTOs.Dashboard
{
    /// <summary>
    /// Represents a ticket displayed on the dashboard.
    /// </summary>
    public class RecentTicketDto
    {
        public int TicketId { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string AssignedTechnicianName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public bool IsClaimed { get; set; }

        public bool HasJobCard { get; set; }
    }
}
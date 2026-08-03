namespace IThelpdesk.DTOs.JobCard
{
    public class JobCardAuditDto
    {
        public int AuditId { get; set; }

        public string Action { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string PerformedBy { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }
    }
}
namespace IThelpdesk.DTOs.JobCard
{
    public class JobCardResponseDto
    {
        public int JobCardId { get; set; }

        public string JobNumber { get; set; } = string.Empty;

        public int TicketId { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }
    }
}
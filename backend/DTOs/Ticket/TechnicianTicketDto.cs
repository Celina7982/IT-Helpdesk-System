namespace IThelpdesk.DTOs.Ticket
{
    public class TechnicianTicketDto
    {
        public int TicketId { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public bool IsEscalated { get; set; }

        public bool HasJobCard { get; set; }

        public int? JobCardId { get; set; }
    }
}

namespace IThelpdesk.DTOs.Ticket
{
    public class TicketDto
    {
        public int TicketId { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
    }
}
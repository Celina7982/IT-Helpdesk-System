namespace IThelpdesk.DTOs.Ticket
{
    public class TicketDetailsDto
    {
        public int TicketId { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public string? AssignedTechnician { get; set; }

        public bool IsEscalated { get; set; }

        public string? EscalationReason { get; set; }
    }
}
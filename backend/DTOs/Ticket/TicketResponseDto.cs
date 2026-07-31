namespace IThelpdesk.DTOs.Ticket
{
    public class TicketResponseDto
    {
        public int TicketId { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public bool IsEscalated { get; set; }

        // NEW
        public string? AssignedTechnician { get; set; }

        // NEW
        public string Category { get; set; } = string.Empty;
    }
}
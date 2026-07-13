using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.Ticket
{
    public class CreateTicketDto
    {
        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public string Priority { get; set; } = string.Empty;
    }
}

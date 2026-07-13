using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.Ticket
{
    public class UpdateTicketStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}

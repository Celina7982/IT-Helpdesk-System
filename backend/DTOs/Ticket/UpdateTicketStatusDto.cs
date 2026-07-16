using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.Tickets
{
    public class UpdateTicketStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
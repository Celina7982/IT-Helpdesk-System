using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs
{
    public class AddTicketAssigneeRequest
    {
        [Required]
        public int UserId { get; set; }
    }
}

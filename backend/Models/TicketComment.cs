using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.Models
{
    public class TicketComment
    {
        [Key]
        public int CommentId { get; set; }

        public int TicketId { get; set; }

        public string AuthorName { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
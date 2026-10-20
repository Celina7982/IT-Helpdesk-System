using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.Models
{
    public class TicketComment
    {
        [Key]
        public int CommentId { get; set; }

        public int TicketId { get; set; }

        // User who created the comment
        public int AuthorUserId { get; set; }

        public string AuthorName { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;


        // false = Public Comment
        // true  = Internal Staff Note
        public bool IsInternal { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
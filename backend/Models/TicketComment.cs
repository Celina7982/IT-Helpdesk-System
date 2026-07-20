using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IThelpdesk.Models
{
    public class TicketComment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(TicketId))]
        public Ticket? Ticket { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
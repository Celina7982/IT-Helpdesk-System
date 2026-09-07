using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IThelpdesk.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        //--------------------------------------------------
        // User who receives the notification
        //--------------------------------------------------

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        //--------------------------------------------------
        // Related Ticket
        //--------------------------------------------------

        public int? TicketId { get; set; }

        //--------------------------------------------------
        // Notification Information
        //--------------------------------------------------

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        //--------------------------------------------------
        // Read Status
        //--------------------------------------------------

        public bool IsRead { get; set; } = false;

        //--------------------------------------------------
        // Date
        //--------------------------------------------------

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
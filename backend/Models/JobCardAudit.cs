using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IThelpdesk.Enums;

namespace IThelpdesk.Models
{
    public class JobCardAudit
    {
        [Key]
        public int AuditId { get; set; }

        //----------------------------------------------------
        // Relationships
        //----------------------------------------------------

        public int JobCardId { get; set; }

        [ForeignKey(nameof(JobCardId))]
        public JobCard JobCard { get; set; } = null!;

        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        //----------------------------------------------------
        // Audit Information
        //----------------------------------------------------

        public JobCardAuditAction Action { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? OldValue { get; set; }

        [MaxLength(2000)]
        public string? NewValue { get; set; }

        //----------------------------------------------------
        // Date
        //----------------------------------------------------

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
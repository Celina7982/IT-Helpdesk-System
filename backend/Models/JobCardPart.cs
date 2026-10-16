using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IThelpdesk.Models
{
    public class JobCardPart
    {
        [Key]
        public int PartId { get; set; }

        [Required]
        public int JobCardId { get; set; }

        [ForeignKey(nameof(JobCardId))]
        public JobCard JobCard { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string PartName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        // Updated: Changed from int to string? to match nvarchar in SQL
        public string? CreatedByUserId { get; set; }

        public DateTime DateAdded { get; set; }

        // Identifies a part entry recorded for a Paper Job Card
        // Digital Job Cards continue using existing parts functionality
        public bool IsPaperPartEntry { get; set; } = false;

        // Admin or Technician responsible for using this part
        // Nullable to preserve existing Digital Job Card parts
        public int? ResponsibleTechnicianId { get; set; }

        // User who captured this part entry
        // May be different from the responsible technician
        public int? CapturedByUserId { get; set; }
    }
}
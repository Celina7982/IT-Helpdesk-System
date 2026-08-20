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

        // NEW: who added the part
        public int CreatedByUserId { get; set; }

        // NEW: when the part was added
        public DateTime DateAdded { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IThelpdesk.Models
{
    public class JobCardLabour
    {
        [Key]
        public int LabourId { get; set; }

        [Required]
        public int JobCardId { get; set; }

        [ForeignKey(nameof(JobCardId))]
        public JobCard JobCard { get; set; } = null!;

        [Required]
        public int TechnicianId { get; set; }

        [ForeignKey(nameof(TechnicianId))]
        public User Technician { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal HoursWorked { get; set; }

        [Required]
        [StringLength(2000)]
        public string WorkPerformed { get; set; } = string.Empty;

        public DateTime DateWorked { get; set; } = DateTime.UtcNow;
    }
}
using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.JobCard
{
    public class AddLabourEntryDto
    {
        [Required]
        public int TechnicianId { get; set; }

        [Required]
        public decimal HoursWorked { get; set; }

        [Required]
        public string WorkPerformed { get; set; } = string.Empty;
    }
}
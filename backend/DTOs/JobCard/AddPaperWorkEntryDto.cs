using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.JobCard
{
    public class AddPaperWorkEntryDto
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a responsible technician.")]
        public int TechnicianId { get; set; }

        [Required(ErrorMessage = "Work description is required.")]
        [StringLength(2000)]
        public string WorkPerformed { get; set; } = string.Empty;
    }
}
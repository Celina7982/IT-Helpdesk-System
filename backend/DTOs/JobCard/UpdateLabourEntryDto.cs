using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.JobCard
{
    /// <summary>
    /// DTO used when updating an existing labour entry.
    /// </summary>
    public class UpdateLabourEntryDto
    {
        //--------------------------------------------------
        // Hours Worked
        //--------------------------------------------------

        [Required]
        [Range(
            0.01,
            9999,
            ErrorMessage = "Hours worked must be greater than 0.")]
        public decimal HoursWorked { get; set; }

        //--------------------------------------------------
        // Work Performed
        //--------------------------------------------------

        [Required]
        [StringLength(
            2000,
            ErrorMessage = "Work performed cannot exceed 2000 characters.")]
        public string WorkPerformed { get; set; } = string.Empty;

        //--------------------------------------------------
        // Date Worked
        //--------------------------------------------------

        [Required]
        public DateTime DateWorked { get; set; }
    }
}
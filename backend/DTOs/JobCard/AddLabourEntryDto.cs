using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.JobCard
{
    /// <summary>
    /// DTO used when adding a labour entry to a Job Card.
    /// The authenticated user is determined from the JWT
    /// and is stored as CreatedByUserId by the service.
    /// </summary>
    public class AddLabourEntryDto
    {
        [Required]
        [Range(
            0.01,
            9999,
            ErrorMessage = "Hours worked must be greater than 0.")]
        public decimal HoursWorked { get; set; }

        [Required]
        [StringLength(
            2000,
            ErrorMessage = "Work performed cannot exceed 2000 characters.")]
        public string WorkPerformed { get; set; } = string.Empty;
    }
}
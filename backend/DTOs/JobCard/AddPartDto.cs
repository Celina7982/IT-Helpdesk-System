using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.JobCard
{
    /// <summary>
    /// DTO used when adding a part to a Job Card.
    /// </summary>
    public class AddPartDto
    {
        [Required]
        [StringLength(
            150,
            ErrorMessage = "Part name cannot exceed 150 characters.")]
        public string PartName { get; set; } = string.Empty;

        [Required]
        [Range(
            1,
            9999,
            ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }
    }
}
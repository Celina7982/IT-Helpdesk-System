using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.JobCard
{
    /// <summary>
    /// DTO used when adding a part to a Job Card.
    /// </summary>
    public class AddPartDto
    {
        [Required]
        [StringLength(150)]
        public string PartName { get; set; } = string.Empty;

        [Required]
        [Range(1, 9999)]
        public int Quantity { get; set; }
    }
}

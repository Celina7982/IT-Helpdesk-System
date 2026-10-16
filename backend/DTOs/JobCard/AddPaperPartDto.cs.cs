
using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.JobCard
{
    public class AddPaperPartDto
    {
        //--------------------------------------------------
        // Part name or description
        //--------------------------------------------------

        [Required(ErrorMessage = "Part name is required.")]
        [StringLength(150)]
        public string PartName { get; set; } = string.Empty;

        //--------------------------------------------------
        // Quantity used
        //--------------------------------------------------

        [Range(1, 9999,
            ErrorMessage = "Quantity must be between 1 and 9999.")]
        public int Quantity { get; set; }

        //--------------------------------------------------
        // Admin or Technician responsible for the part
        //--------------------------------------------------

        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a responsible technician.")]
        public int ResponsibleTechnicianId { get; set; }
    }
}


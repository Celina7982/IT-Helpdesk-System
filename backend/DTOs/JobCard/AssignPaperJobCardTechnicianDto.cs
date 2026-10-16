using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.JobCard
{
    public class AssignPaperJobCardTechnicianDto
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Please select a valid technician.")]
        public int TechnicianId { get; set; }

        public bool IsLeadTechnician { get; set; } = false;
    }
}
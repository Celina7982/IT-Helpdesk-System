
namespace IThelpdesk.DTOs.JobCard
{
    public class JobCardTechnicianDto
    {
        public int TechnicianId { get; set; }

        public string TechnicianName { get; set; }
            = string.Empty;

        public bool IsLeadTechnician { get; set; }

        public DateTime DateAssigned { get; set; }
    }
}

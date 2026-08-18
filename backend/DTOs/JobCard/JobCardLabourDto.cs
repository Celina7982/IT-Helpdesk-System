

namespace IThelpdesk.DTOs.JobCard
{
    public class JobCardLabourDto
    {
        public int LabourId { get; set; }
        public int JobCardId { get; set; }
        public decimal HoursWorked { get; set; }
        public string WorkPerformed { get; set; } = string.Empty;
        public DateTime DateWorked { get; set; }
        public int CreatedByUserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
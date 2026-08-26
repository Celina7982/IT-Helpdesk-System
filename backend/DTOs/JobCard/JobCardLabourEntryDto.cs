namespace IThelpdesk.DTOs.JobCard
{
    /// <summary>
    /// DTO returned when viewing Labour Entries on a Job Card.
    /// </summary>
    public class JobCardLabourEntryDto
    {
        public int LabourId { get; set; }

        public int JobCardId { get; set; }

        public int TechnicianId { get; set; }

        public string TechnicianName { get; set; } = string.Empty;

        public decimal HoursWorked { get; set; }

        public string WorkPerformed { get; set; } = string.Empty;

        public DateTime DateWorked { get; set; }
    }
}
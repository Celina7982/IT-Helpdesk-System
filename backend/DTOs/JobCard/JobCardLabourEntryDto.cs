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

        //--------------------------------------------------
        // Paper Job Card - Work Completed Information
        //--------------------------------------------------

        // User who captured the work entry
        public int? CapturedByUserId { get; set; }

        // Display name of the person who captured the entry
        public string? CapturedByUserName { get; set; }

        // Identifies Paper Job Card work entries
        public bool IsPaperWorkEntry { get; set; }
    }
}
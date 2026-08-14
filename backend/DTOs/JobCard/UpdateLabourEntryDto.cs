namespace IThelpdesk.DTOs.JobCard
{
    public class UpdateLabourEntryDto
    {
        public decimal HoursWorked { get; set; }

        public string WorkPerformed { get; set; } = string.Empty;

        public DateTime DateWorked { get; set; }
    }
}
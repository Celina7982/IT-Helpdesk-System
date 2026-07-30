namespace IThelpdesk.DTOs.JobCard
{
    public class JobCardDetailsDto
    {
        public int JobCardId { get; set; }

        public string JobNumber { get; set; } = string.Empty;

        public int TicketId { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }

        public DateTime? DateCompleted { get; set; }


        public int? AssignedTechnicianId { get; set; }
        // Technician assigned to this Job Card
        public string AssignedTechnician { get; set; } = "Not Assigned";

        // Work Information
        public string FaultReported { get; set; } = string.Empty;

        public string FaultFound { get; set; } = string.Empty;

        public string WorkPerformed { get; set; } = string.Empty;

        public string CompletionNotes { get; set; } = string.Empty;

        // Customer Acceptance
        public string CustomerName { get; set; } = string.Empty;

        public string CustomerSignature { get; set; } = string.Empty;

        public DateTime? SignedDate { get; set; }
    }
}
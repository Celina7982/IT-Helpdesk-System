
using IThelpdesk.Enums;


namespace IThelpdesk.DTOs.JobCard
{
    public class JobCardDetailsDto
    {
        public int JobCardId { get; set; }

        public string JobNumber { get; set; } = string.Empty;


        public int? TicketId { get; set; } // Ticket ID == optional for Paper Job Cards


        //--------------------------------------------------
        // Job Card Type
        //--------------------------------------------------

        public JobCardType JobCardType { get; set; }

        //--------------------------------------------------
        // Paper Job Card Information
        //--------------------------------------------------

        public string? PaperJobCardNumber { get; set; }

        public DateTime? PaperJobCardDate { get; set; }

        public int? CapturedByUserId { get; set; }

        public string? CapturedByUserName { get; set; }

        public DateTime? DateCaptured { get; set; }


        //--------------------------------------------------
        // Paper Job Card - Service Type
        //--------------------------------------------------

        public JobCardServiceType? ServiceType { get; set; }

        //--------------------------------------------------
        // Paper Job Card - Overall Time Tracking
        //--------------------------------------------------

        public DateTimeOffset? TimeIn { get; set; }

        public DateTimeOffset? TimeOut { get; set; }

        //--------------------------------------------------
        // Paper Job Card - Travel and On Site
        //--------------------------------------------------

        public bool? IsTravel { get; set; }

        public bool? IsOnSite { get; set; }


        public string Status { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }

        public DateTime? DateCompleted { get; set; }

        public int? AssignedTechnicianId { get; set; }
        // Technician assigned to this Job Card
        public string AssignedTechnician { get; set; } = "Not Assigned";


        //--------------------------------------------------
        // Paper Job Card - Multiple Assigned Technicians
        //--------------------------------------------------

        public List<JobCardTechnicianDto> AssignedTechnicians
        { get; set; } = new();


        // Work Information
        public string FaultReported { get; set; } = string.Empty;

        public string FaultFound { get; set; } = string.Empty;

        public string WorkPerformed { get; set; } = string.Empty;

        public string CompletionNotes { get; set; } = string.Empty;

        // Customer Acceptance
        public string CustomerName { get; set; } = string.Empty;

        public string CustomerSignature { get; set; } = string.Empty;

        public DateTime? SignedDate { get; set; }

        // Navigation collections for PDF & Detail views
        public List<JobCardLabourEntryDto> LabourEntries { get; set; } = new();

        public List<JobCardPartDto> Parts { get; set; } = new();
    }
}
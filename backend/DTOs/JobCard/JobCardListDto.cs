
using IThelpdesk.Enums;



namespace IThelpdesk.DTOs.JobCard
{
  
    /// Lightweight DTO used when displaying a list of Job Cards.
    
    public class JobCardListDto
    {
        //--------------------------------------------------
        // Identification
        //--------------------------------------------------

        public int JobCardId { get; set; }

        public string JobNumber { get; set; } = string.Empty;

        //--------------------------------------------------
        // Ticket Information
        //--------------------------------------------------



        public int? TicketId { get; set; }


        //--------------------------------------------------
        // Job Card Type
        //--------------------------------------------------

        public JobCardType JobCardType { get; set; }

        //--------------------------------------------------
        // Paper Job Card Information
        //--------------------------------------------------

        public string? PaperJobCardNumber { get; set; }

        public DateTime? PaperJobCardDate { get; set; }



        public string CustomerName { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        //--------------------------------------------------
        // Technician
        //--------------------------------------------------

        public int? AssignedTechnicianId { get; set; }

        public string AssignedTechnicianName { get; set; } = string.Empty;

        //--------------------------------------------------
        // Status
        //--------------------------------------------------

        public string Status { get; set; } = string.Empty;

        //--------------------------------------------------
        // Dates
        //--------------------------------------------------

        public DateTime DateCreated { get; set; }

        public DateTime? DateCompleted { get; set; }
    }
}
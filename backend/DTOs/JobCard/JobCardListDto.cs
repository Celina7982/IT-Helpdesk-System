namespace IThelpdesk.DTOs.JobCard
{
    /// <summary>
    /// Lightweight DTO used when displaying a list of Job Cards.
    ///
    /// This DTO intentionally excludes detailed information such as
    /// work performed, labour entries, parts used and customer
    /// signatures to reduce the amount of data transferred between
    /// the API and the frontend.
    /// </summary>
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

        public int TicketId { get; set; }

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

using System;
using IThelpdesk.Enums;

namespace IThelpdesk.DTOs.JobCard
{
    public class JobCardResponseDto
    {
        //--------------------------------------------------
        // Identification
        //--------------------------------------------------

        public int JobCardId { get; set; }

        public string JobNumber { get; set; } = string.Empty;

        //--------------------------------------------------
        // Job Card Type
        //--------------------------------------------------

        public JobCardType JobCardType { get; set; }
            = JobCardType.Digital;

        //--------------------------------------------------
        // Ticket Information
        // Optional for Paper Job Cards
        //--------------------------------------------------

        public int? TicketId { get; set; }

        //--------------------------------------------------
        // Paper Job Card Information
        //--------------------------------------------------

        public string? PaperJobCardNumber { get; set; }

        public DateTime? PaperJobCardDate { get; set; }

        //--------------------------------------------------
        // Status
        //--------------------------------------------------

        public string Status { get; set; } = string.Empty;

        //--------------------------------------------------
        // Dates
        //--------------------------------------------------

        public DateTime DateCreated { get; set; }
    }
}

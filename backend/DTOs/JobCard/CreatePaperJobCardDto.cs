
using System;
using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.JobCard
{
    public class CreatePaperJobCardDto
    {
        //--------------------------------------------------
        // Original Paper Job Card Information
        //--------------------------------------------------

        [Required(ErrorMessage = "Paper job card number is required.")]
        [MaxLength(50)]
        public string PaperJobCardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Paper job card date is required.")]
        public DateTime? PaperJobCardDate { get; set; }

        //--------------------------------------------------
        // Optional Ticket
        //--------------------------------------------------

        public int? TicketId { get; set; }

        //--------------------------------------------------
        // Customer Information
        //--------------------------------------------------

        [Required(ErrorMessage = "Customer name is required.")]
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        //--------------------------------------------------
        // Technician Assignment
        //--------------------------------------------------

        public int? AssignedTechnicianId { get; set; }


        //--------------------------------------------------
        // All technicians assigned during registration
        // Includes the Lead Technician
        //--------------------------------------------------

        public List<int> AssignedTechnicianIds { get; set; } = new();


        //--------------------------------------------------
        // Work Information
        //--------------------------------------------------

        [Required(ErrorMessage = "Fault reported is required.")]
        [MaxLength(2000)]
        public string FaultReported { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string FaultFound { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string WorkPerformed { get; set; } = string.Empty;

        public string CompletionNotes { get; set; } = string.Empty;


        //--------------------------------------------------
        // Unique identifier for a Paper Job Card registration
        //--------------------------------------------------

        public Guid? RegistrationId { get; set; }
    }
}


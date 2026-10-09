using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IThelpdesk.Models
{
    public class SlaTicket
    {
        //--------------------------------------------------
        // Identification
        //--------------------------------------------------

        [Key]
        public int SlaTicketId { get; set; }

        [Required]
        [StringLength(50)]
        public string SlaNumber { get; set; } = string.Empty;


        //--------------------------------------------------
        // Original Ticket
        //--------------------------------------------------

        [Required]
        public int TicketId { get; set; }

        [ForeignKey(nameof(TicketId))]
        public Ticket? Ticket { get; set; }


        //--------------------------------------------------
        // Technician
        //--------------------------------------------------

        public int? TechnicianId { get; set; }

        [ForeignKey(nameof(TechnicianId))]
        public User? Technician { get; set; }


        //--------------------------------------------------
        // Customer Information
        //--------------------------------------------------

        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [StringLength(255)]
        public string CustomerEmail { get; set; } = string.Empty;


        //--------------------------------------------------
        // Service Information
        //--------------------------------------------------

        [StringLength(2000)]
        public string Issue { get; set; } = string.Empty;

        [StringLength(4000)]
        public string WorkPerformed { get; set; } = string.Empty;

        [StringLength(4000)]
        public string ResolutionNotes { get; set; } = string.Empty;


        //--------------------------------------------------
        // Status
        //--------------------------------------------------

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Open";


        //--------------------------------------------------
        // Dates
        //--------------------------------------------------

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateCompleted { get; set; }


        //--------------------------------------------------
        // Created By
        //--------------------------------------------------

        public int CreatedByUserId { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public User? CreatedByUser { get; set; }


        //--------------------------------------------------
        // Email Tracking
        //--------------------------------------------------

        public DateTime? EmailedDate { get; set; }

        [StringLength(255)]
        public string RecipientEmail { get; set; } = string.Empty;

        public int? EmailedByUserId { get; set; }

        [ForeignKey(nameof(EmailedByUserId))]
        public User? EmailedByUser { get; set; }
    }
}

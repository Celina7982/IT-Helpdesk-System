using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IThelpdesk.Models
{
    public class JobCard
    {
        [Key]
        public int JobCardId { get; set; }

        [Required]
        [MaxLength(30)]
        public string JobNumber { get; set; } = string.Empty;

        //--------------------------------------------------
        // Ticket
        //--------------------------------------------------

        public int TicketId { get; set; }

        public Ticket? Ticket { get; set; }

        //--------------------------------------------------
        // Technician
        //--------------------------------------------------

        public int? AssignedTechnicianId { get; set; }

        [ForeignKey(nameof(AssignedTechnicianId))]
        public User? AssignedTechnician { get; set; }

        //--------------------------------------------------
        // Status
        //--------------------------------------------------

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Open";

        //--------------------------------------------------
        // Dates
        //--------------------------------------------------

        public DateTime DateCreated { get; set; }

        public DateTime? DateCompleted { get; set; }

        //--------------------------------------------------
        // Work Information
        //--------------------------------------------------

        [MaxLength(2000)]
        public string FaultReported { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string FaultFound { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string WorkPerformed { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string CompletionNotes { get; set; } = string.Empty;

        //--------------------------------------------------
        // Customer Acceptance
        //--------------------------------------------------

        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string CustomerSignature { get; set; } = string.Empty;

        public DateTime? SignedDate { get; set; }

        //--------------------------------------------------
        // Navigation
        //--------------------------------------------------

        public ICollection<JobCardLabour> LabourEntries { get; set; }
            = new List<JobCardLabour>();

        public ICollection<JobCardPart> PartsUsed { get; set; }
            = new List<JobCardPart>();
    }
}
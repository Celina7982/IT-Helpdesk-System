using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

using IThelpdesk.Enums;


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
        // Job Card Type
        //--------------------------------------------------

        public JobCardType JobCardType { get; set; } = JobCardType.Digital;



        //--------------------------------------------------
        // Service Type
        //--------------------------------------------------

        public JobCardServiceType? ServiceType { get; set; }



        //--------------------------------------------------
        // Overall Job Time Tracking
        //--------------------------------------------------

        public DateTimeOffset? TimeIn { get; set; }

        public DateTimeOffset? TimeOut { get; set; }




        //--------------------------------------------------
        // Paper Job Card - Travel and On Site
        //--------------------------------------------------

        public bool? IsTravel { get; set; }

        public bool? IsOnSite { get; set; }


        //--------------------------------------------------
        // Paper Job Card Information
        //--------------------------------------------------

        [MaxLength(50)]
        public string? PaperJobCardNumber { get; set; }

        public DateTime? PaperJobCardDate { get; set; }

        //--------------------------------------------------
        // Paper Job Card Registration Idempotency
        //--------------------------------------------------

        public Guid? RegistrationId { get; set; }
        //--------------------------------------------------
        // Original Paper Job Card Registration Fingerprint
        //--------------------------------------------------

        [MaxLength(64)]
        public string? RegistrationPayloadHash { get; set; }


        public int? CapturedByUserId { get; set; }

        [ForeignKey(nameof(CapturedByUserId))]
        public User? CapturedByUser { get; set; }

        public DateTime? DateCaptured { get; set; }



        //--------------------------------------------------
        // Ticket (Optional for Paper Job Cards)
        //--------------------------------------------------

        public int? TicketId { get; set; }

        [ForeignKey(nameof(TicketId))]
        public Ticket? Ticket { get; set; }


        //--------------------------------------------------
        // Technician
        //--------------------------------------------------

        public int? AssignedTechnicianId { get; set; }

        [ForeignKey(nameof(AssignedTechnicianId))]
        public User? AssignedTechnician { get; set; }




        //--------------------------------------------------
        // Multiple Technicians - Paper Job Cards
        //--------------------------------------------------

        public ICollection<JobCardTechnician> JobCardTechnicians
        { get; set; } = new List<JobCardTechnician>();

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

        //--------------------------------------------------
        // Navigation Properties
        //--------------------------------------------------

        public ICollection<JobCardAudit> AuditHistory { get; set; }
            = new List<JobCardAudit>();
    }
}
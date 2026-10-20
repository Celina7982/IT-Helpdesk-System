using System;

////THIS CODE SUPPORTS multiple technicians assigned to one Paper Job Card,

namespace IThelpdesk.Models
{
    public class JobCardTechnician
    {
        // Primary key
        public int JobCardTechnicianId { get; set; }

        // Job card this technician is assigned to
        public int JobCardId { get; set; }

        public JobCard JobCard { get; set; } = null!;

        // Technician assigned to this job card
        public int TechnicianId { get; set; }

        public User Technician { get; set; } = null!;

        // Identifies the lead technician
        public bool IsLeadTechnician { get; set; }

        // User who assigned this technician
        public int AssignedByUserId { get; set; }

        public User AssignedByUser { get; set; } = null!;

        // Date assignment was created
        public DateTime DateAssigned { get; set; } = DateTime.UtcNow;
    }
}

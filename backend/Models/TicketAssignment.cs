using IThelpdesk.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IThelpdesk.Entities
{
    public class TicketAssignment
    {
        [Key]
        public int TicketAssignmentId { get; set; }

        // ----------------------------------------------------
        // Ticket being assigned
        // ----------------------------------------------------

        [Required]
        public int TicketId { get; set; }

        [ForeignKey(nameof(TicketId))]
        public Ticket Ticket { get; set; } = null!;


        // ----------------------------------------------------
        // User assigned to the ticket
        // Can be an Admin or Technician
        // ----------------------------------------------------

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;


        // ----------------------------------------------------
        // Admin who made the assignment
        // ----------------------------------------------------

        [Required]
        public int AssignedByUserId { get; set; }

        [ForeignKey(nameof(AssignedByUserId))]
        public User AssignedByUser { get; set; } = null!;


        // ----------------------------------------------------
        // Assignment information
        // ----------------------------------------------------

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    }
}
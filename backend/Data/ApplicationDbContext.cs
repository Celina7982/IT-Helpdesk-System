using IThelpdesk.Entities;
using IThelpdesk.Models;
using Microsoft.EntityFrameworkCore;
 

namespace IThelpdesk.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //--------------------------------------------------
        // Existing Tables
        //--------------------------------------------------

        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<TicketAssignment> TicketAssignments { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }
        //--------------------------------------------------
        // Job Cards
        //--------------------------------------------------

        public DbSet<JobCard> JobCards { get; set; }
        public DbSet<JobCardLabour> JobCardLabours { get; set; }

        public DbSet<JobCardPart> JobCardParts { get; set; }

        //SLA Tickets
        public DbSet<SlaTicket> SlaTickets { get; set; }

        //Notifications
        public DbSet<Notification> Notifications { get; set; }

        //--------------------------------------------------
        // Job Card Audit History
        //--------------------------------------------------

        public DbSet<JobCardAudit> JobCardAudits { get; set; }

        //--------------------------------------------------
        // Model Configuration
        //--------------------------------------------------

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //--------------------------------------------------
            // Ticket -> User
            //--------------------------------------------------

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //--------------------------------------------------
            // Ticket -> Assigned Technician
            //--------------------------------------------------

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedToUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // ========================================================
            // TICKET ASSIGNMENTS
            // Allows multiple Admins / Technicians on one ticket
            // ========================================================

            modelBuilder.Entity<TicketAssignment>()
                .HasOne(a => a.Ticket)
                .WithMany(t => t.Assignments)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);


            // Assigned user
            modelBuilder.Entity<TicketAssignment>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            // Admin who performed the assignment
            modelBuilder.Entity<TicketAssignment>()
                .HasOne(a => a.AssignedByUser)
                .WithMany()
                .HasForeignKey(a => a.AssignedByUserId)
                .OnDelete(DeleteBehavior.NoAction);


            // Prevent the same user from being assigned
            // to the same ticket more than once
            modelBuilder.Entity<TicketAssignment>()
                .HasIndex(a => new
                {
                    a.TicketId,
                    a.UserId
                })
                .IsUnique();


            // Improves "My Tickets" lookup performance
            modelBuilder.Entity<TicketAssignment>()
                .HasIndex(a => a.UserId);

            //--------------------------------------------------
            // Job Card -> Ticket
            //--------------------------------------------------

            modelBuilder.Entity<JobCard>()
                .HasOne(j => j.Ticket)
                .WithMany()
                .HasForeignKey(j => j.TicketId);


            //--------------------------------------------------
            // SLA Ticket -> Ticket
            //--------------------------------------------------

            modelBuilder.Entity<SlaTicket>()
                .HasOne(s => s.Ticket)
                .WithMany()
                .HasForeignKey(s => s.TicketId)
                .OnDelete(DeleteBehavior.NoAction);

            //--------------------------------------------------
            // SLA Ticket -> Technician
            //--------------------------------------------------

            modelBuilder.Entity<SlaTicket>()
                .HasOne(s => s.Technician)
                .WithMany()
                .HasForeignKey(s => s.TechnicianId)
                .OnDelete(DeleteBehavior.NoAction);

            //--------------------------------------------------
            // SLA Ticket -> Created By User
            //--------------------------------------------------

            modelBuilder.Entity<SlaTicket>()
                .HasOne(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            //--------------------------------------------------
            // SLA Ticket -> Emailed By User
            //--------------------------------------------------

            modelBuilder.Entity<SlaTicket>()
                .HasOne(s => s.EmailedByUser)
                .WithMany()
                .HasForeignKey(s => s.EmailedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            //--------------------------------------------------
            // Job Card -> Labour Entries
            //--------------------------------------------------

            modelBuilder.Entity<JobCardLabour>()
                .HasOne(l => l.JobCard)
                .WithMany(j => j.LabourEntries)
                .HasForeignKey(l => l.JobCardId);

            //--------------------------------------------------
            // Labour Entry -> Technician
            //--------------------------------------------------

            modelBuilder.Entity<JobCardLabour>()
                .HasOne(l => l.Technician)
                .WithMany()
                .HasForeignKey(l => l.TechnicianId)
                .OnDelete(DeleteBehavior.NoAction);

            //--------------------------------------------------
            // Job Card -> Parts Used
            //--------------------------------------------------

            modelBuilder.Entity<JobCardPart>()
                .HasOne(p => p.JobCard)
                .WithMany(j => j.PartsUsed)
                .HasForeignKey(p => p.JobCardId);

            //--------------------------------------------------
            // Job Card Audit -> Job Card
            //--------------------------------------------------

            modelBuilder.Entity<JobCardAudit>()
                .HasOne(a => a.JobCard)
                .WithMany(j => j.AuditHistory)
                .HasForeignKey(a => a.JobCardId)
                .OnDelete(DeleteBehavior.Cascade);

            //--------------------------------------------------
            // Job Card Audit -> User
            //--------------------------------------------------

            modelBuilder.Entity<JobCardAudit>()
                .HasOne(a => a.User)
                .WithMany(u => u.JobCardAudits)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //---------------------------------------
            // Job Card Audit Indexes
            //---------------------------------------

            modelBuilder.Entity<JobCardAudit>()
                .HasIndex(a => a.JobCardId);

            modelBuilder.Entity<JobCardAudit>()
                .HasIndex(a => a.DateCreated);

            modelBuilder.Entity<JobCardAudit>()
                .HasIndex(a => new
                {
                    a.JobCardId,
                    a.DateCreated
                });

            //---------------------------------------
            // Ticket Indexes
            //---------------------------------------

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => new
                {
                    t.AssignedToUserId,
                    t.IsArchived,
                    t.CreatedDate
                })
                .HasDatabaseName("IX_Tickets_AssignedToUserId_IsArchived_CreatedDate");

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => new
                {
                    t.IsArchived,
                    t.CreatedDate
                })
                .HasDatabaseName("IX_Tickets_IsArchived_CreatedDate");

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => new
                {
                    t.UserId,
                    t.IsArchived,
                    t.CreatedDate
                })
                .HasDatabaseName("IX_Tickets_UserId_IsArchived_CreatedDate");

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => new
                {
                    t.IsEscalated,
                    t.Status,
                    t.IsArchived,
                    t.CreatedDate
                })
                .HasDatabaseName("IX_Tickets_Escalation_Status_IsArchived_CreatedDate");

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => new
                {
                    t.IsArchived,
                    t.ArchivedDate
                })
                .HasDatabaseName("IX_Tickets_IsArchived_ArchivedDate");



            //---------------------------------------
            // Job Card Indexes
            //---------------------------------------

            modelBuilder.Entity<JobCard>()
                .HasIndex(j => j.TicketId)
                .HasDatabaseName("IX_JobCards_TicketId");

            modelBuilder.Entity<JobCard>()
                .HasIndex(j => j.AssignedTechnicianId)
                .HasDatabaseName("IX_JobCards_AssignedTechnicianId");


            //---------------------------------------
            // Job Card Labour Indexes
            //---------------------------------------

            modelBuilder.Entity<JobCardLabour>()
                .HasIndex(l => l.JobCardId)
                .HasDatabaseName("IX_JobCardLabours_JobCardId");

            modelBuilder.Entity<JobCardLabour>()
                .HasIndex(l => l.TechnicianId)
                .HasDatabaseName("IX_JobCardLabours_TechnicianId");



            //---------------------------------------
            // Job Card Part Indexes
            //---------------------------------------

            modelBuilder.Entity<JobCardPart>()
                .HasIndex(p => p.JobCardId)
                .HasDatabaseName("IX_JobCardParts_JobCardId");

            //--------------------------------------------------
            // SLA Ticket Indexes
            //--------------------------------------------------

            // A Ticket can only have ONE SLA Report
            modelBuilder.Entity<SlaTicket>()
                .HasIndex(s => s.TicketId)
                .IsUnique()
                .HasDatabaseName("IX_SlaTickets_TicketId");

            // SLA numbers must be unique
            modelBuilder.Entity<SlaTicket>()
                .HasIndex(s => s.SlaNumber)
                .IsUnique()
                .HasDatabaseName("IX_SlaTickets_SlaNumber");

            // Helps when finding SLA reports assigned to a technician
            modelBuilder.Entity<SlaTicket>()
                .HasIndex(s => s.TechnicianId)
                .HasDatabaseName("IX_SlaTickets_TechnicianId");


            //---------------------------------------
            // Notification Indexes
            //---------------------------------------

            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.TicketId)
                .HasDatabaseName("IX_Notifications_TicketId");

            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.UserId)
                .HasDatabaseName("IX_Notifications_UserId");

            

            //---------------------------------------
            // User Indexes
            //---------------------------------------

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

        }

    }
}
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
        public DbSet<TicketComment> TicketComments { get; set; }
        //--------------------------------------------------
        // Job Cards
        //--------------------------------------------------

        public DbSet<JobCard> JobCards { get; set; }
        public DbSet<JobCardLabour> JobCardLabours { get; set; }

        public DbSet<JobCardPart> JobCardParts { get; set; }

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

            //--------------------------------------------------
            // Job Card -> Ticket
            //--------------------------------------------------

            modelBuilder.Entity<JobCard>()
                .HasOne(j => j.Ticket)
                .WithMany()
                .HasForeignKey(j => j.TicketId);

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
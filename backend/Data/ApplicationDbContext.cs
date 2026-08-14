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

        }
    }
}
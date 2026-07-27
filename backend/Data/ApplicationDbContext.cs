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

        //---------------------------------------
        // Existing Tables
        //---------------------------------------

        public DbSet<User> Users { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        //---------------------------------------
        // Sprint 2 - Job Cards
        //---------------------------------------

        public DbSet<JobCard> JobCards { get; set; }

        public DbSet<JobCardLabour> JobCardLabours { get; set; }

        public DbSet<JobCardPart> JobCardParts { get; set; }

        //---------------------------------------

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //---------------------------------------
            // Ticket -> User
            //---------------------------------------

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedToUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.NoAction);

            //---------------------------------------
            // Ticket -> Job Card
            //---------------------------------------

            modelBuilder.Entity<JobCard>()
                .HasOne(j => j.Ticket)
                .WithMany()
                .HasForeignKey(j => j.TicketId);

            //---------------------------------------
            // Job Card -> Labour
            //---------------------------------------

            modelBuilder.Entity<JobCardLabour>()
                .HasOne(l => l.JobCard)
                .WithMany(j => j.LabourEntries)
                .HasForeignKey(l => l.JobCardId);


            modelBuilder.Entity<JobCardLabour>()
    .HasOne(l => l.Technician)
    .WithMany()
    .HasForeignKey(l => l.TechnicianId)
    .OnDelete(DeleteBehavior.NoAction);

            //---------------------------------------
            // Job Card -> Parts
            //---------------------------------------

            modelBuilder.Entity<JobCardPart>()
                .HasOne(p => p.JobCard)
                .WithMany(j => j.PartsUsed)
                .HasForeignKey(p => p.JobCardId);
        }
    }
}
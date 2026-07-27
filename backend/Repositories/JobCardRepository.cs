using Microsoft.EntityFrameworkCore;
using IThelpdesk.Data;
using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Models;

namespace IThelpdesk.Repositories
{
    public class JobCardRepository : IJobCardRepository
    {
        private readonly ApplicationDbContext _context;

        public JobCardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //--------------------------------------------------
        // Get All Job Cards
        //--------------------------------------------------

        public async Task<IEnumerable<JobCard>> GetAllAsync()
        {
            return await _context.JobCards
                .OrderByDescending(j => j.DateCreated)
                .ToListAsync();
        }


        //--------------------------------------------------
        // Get Job Card List
        //--------------------------------------------------

        public async Task<IEnumerable<JobCardListDto>> GetJobCardListAsync()
        {
            return await _context.JobCards

                .OrderByDescending(j => j.DateCreated)

                .Select(j => new JobCardListDto
                {
                    JobCardId = j.JobCardId,

                    JobNumber = j.JobNumber,

                    TicketId = j.TicketId,

                    CustomerName = j.Ticket != null
                        ? j.Ticket.CustomerName
                        : string.Empty,

                    CompanyName = j.Ticket != null
                        ? j.Ticket.CompanyName
                        : string.Empty,

                    Subject = j.Ticket != null
                        ? j.Ticket.Subject
                        : string.Empty,

                    AssignedTechnicianId = j.AssignedTechnicianId,

                    AssignedTechnicianName =
                        j.AssignedTechnician != null
                            ? $"{j.AssignedTechnician.FirstName} {j.AssignedTechnician.LastName}"
                            : "Not Assigned",

                    Status = j.Status,

                    DateCreated = j.DateCreated,

                    DateCompleted = j.DateCompleted
                })

                .ToListAsync();
        }


        //--------------------------------------------------
        // Get Job Card List By Technician
        //--------------------------------------------------

        public async Task<IEnumerable<JobCardListDto>> GetJobCardListByTechnicianAsync(
            int technicianId)
        {
            return await _context.JobCards

                .Where(j => j.AssignedTechnicianId == technicianId)

                .OrderByDescending(j => j.DateCreated)

                .Select(j => new JobCardListDto
                {
                    JobCardId = j.JobCardId,

                    JobNumber = j.JobNumber,

                    TicketId = j.TicketId,

                    CustomerName = j.Ticket != null
                        ? j.Ticket.CustomerName
                        : string.Empty,

                    CompanyName = j.Ticket != null
                        ? j.Ticket.CompanyName
                        : string.Empty,

                    Subject = j.Ticket != null
                        ? j.Ticket.Subject
                        : string.Empty,

                    AssignedTechnicianId = j.AssignedTechnicianId,

                    AssignedTechnicianName =
                        j.AssignedTechnician != null
                            ? $"{j.AssignedTechnician.FirstName} {j.AssignedTechnician.LastName}"
                            : "Not Assigned",

                    Status = j.Status,

                    DateCreated = j.DateCreated,

                    DateCompleted = j.DateCompleted
                })

                .ToListAsync();
        }

        //--------------------------------------------------
        // Get By Id
        //--------------------------------------------------

        public async Task<JobCard?> GetByIdAsync(int id)
        {
            return await _context.JobCards
                .FirstOrDefaultAsync(j => j.JobCardId == id);
        }

        public async Task<JobCard?> GetByTicketIdAsync(int ticketId)
        {
            return await _context.JobCards
                .FirstOrDefaultAsync(j => j.TicketId == ticketId);
        }

        //--------------------------------------------------
        // Get Details
        //--------------------------------------------------

        public async Task<JobCardDetailsDto?> GetDetailsAsync(int id)
        {
            return await _context.JobCards

                .Include(j => j.Ticket)

                .Include(j => j.AssignedTechnician)

                .Where(j => j.JobCardId == id)

                .Select(j => new JobCardDetailsDto
                {
                    JobCardId = j.JobCardId,

                    JobNumber = j.JobNumber,

                    TicketId = j.TicketId,

                    Status = j.Status,

                    DateCreated = j.DateCreated,

                    DateCompleted = j.DateCompleted,

                    FaultReported = j.FaultReported,

                    FaultFound = j.FaultFound,

                    WorkPerformed = j.WorkPerformed,

                    CompletionNotes = j.CompletionNotes,

                    CustomerName = j.CustomerName,

                    CustomerSignature = j.CustomerSignature,

                    SignedDate = j.SignedDate,

                    AssignedTechnician =
                        j.AssignedTechnician != null
                            ? j.AssignedTechnician.FirstName + " " + j.AssignedTechnician.LastName
                            : "Not Assigned"
                })

                .FirstOrDefaultAsync();
        }

        //--------------------------------------------------
        // Add
        //--------------------------------------------------

        public async Task AddAsync(JobCard jobCard)
        {
            await _context.JobCards.AddAsync(jobCard);
        }

        //--------------------------------------------------
        // Update
        //--------------------------------------------------

        public async Task UpdateAsync(JobCard jobCard)
        {
            _context.JobCards.Update(jobCard);

            await Task.CompletedTask;
        }

        //--------------------------------------------------
        // Delete
        //--------------------------------------------------

        public async Task DeleteAsync(JobCard jobCard)
        {
            _context.JobCards.Remove(jobCard);

            await Task.CompletedTask;
        }

        //======================================
        //  job card number sequencing
        //====================================

        public async Task<JobCard?> GetLatestJobCardAsync()
        {
            return await _context.JobCards

                .OrderByDescending(j => j.JobCardId)

                .FirstOrDefaultAsync();
        }



        //--------------------------------------------------
        // Add Labour Entry
        //--------------------------------------------------

        public async Task AddLabourEntryAsync(JobCardLabour labour)
        {
            await _context.JobCardLabours.AddAsync(labour);
        }

        //--------------------------------------------------
        // Get Labour Entries
        //--------------------------------------------------

        public async Task<List<JobCardLabour>> GetLabourEntriesAsync(int jobCardId)
        {
            return await _context.JobCardLabours

                .Include(l => l.Technician)

                .Where(l => l.JobCardId == jobCardId)

                .OrderBy(l => l.DateWorked)

                .ToListAsync();
        }


        //--------------------------------------------------
        // Save
        //--------------------------------------------------

        //public async Task SaveChangesAsync()
        //{
        //    await _context.SaveChangesAsync();
        //}


        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                throw;
            }
        }
        //--------------------------------------------------
        // Check Job Number
        //--------------------------------------------------

        public async Task<bool> JobNumberExistsAsync(string jobNumber)
        {
            return await _context.JobCards
                .AnyAsync(j => j.JobNumber == jobNumber);
        }
    }
}

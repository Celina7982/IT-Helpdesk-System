using IThelpdesk.Data;
using IThelpdesk.DTOs.Common;
using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        // Job Card List
        //--------------------------------------------------

        public async Task<PagedResultDto<JobCardListDto>> GetJobCardListAsync(
           int? technicianId,
            string? status,
            int? assignedTo,
            string? search,
            string? sortBy,
            string? sortDirection,
            int pageNumber,
            int pageSize)
        {
            var query = _context.JobCards
           .Include(j => j.Ticket)
           .Include(j => j.AssignedTechnician)
           .Include(j => j.AuditHistory)
           .AsQueryable();

            //--------------------------------------------------
            // Technician Filter
            //--------------------------------------------------

            if (technicianId.HasValue)
            {
                query = query.Where(j =>
                    j.AssignedTechnicianId == technicianId.Value);
            }

            //--------------------------------------------------
            // Assigned Technician Filter
            //--------------------------------------------------

            if (assignedTo.HasValue)
            {
                query = query.Where(j =>
                    j.AssignedTechnicianId == assignedTo.Value);
            }

            //--------------------------------------------------
            // Status Filter
            //--------------------------------------------------

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(j => j.Status == status);
            }

            //--------------------------------------------------
            // Search
            //--------------------------------------------------

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(j =>

                    j.JobNumber.Contains(search)

                    ||

                    (j.Ticket != null &&

                    (
                        j.Ticket.CustomerName.Contains(search)

                        ||

                        j.Ticket.CompanyName.Contains(search)

                        ||

                        j.Ticket.Subject.Contains(search)
                    )));
            }

            //--------------------------------------------------
            // Sorting
            //--------------------------------------------------

            query = (sortBy?.ToLower(), sortDirection?.ToLower()) switch
            {
                ("jobnumber", "asc") =>
                    query.OrderBy(j => j.JobNumber),

                ("jobnumber", "desc") =>
                    query.OrderByDescending(j => j.JobNumber),

                ("customer", "asc") =>
                    query.OrderBy(j => j.Ticket!.CustomerName),

                ("customer", "desc") =>
                    query.OrderByDescending(j => j.Ticket!.CustomerName),

                ("company", "asc") =>
                    query.OrderBy(j => j.Ticket!.CompanyName),

                ("company", "desc") =>
                    query.OrderByDescending(j => j.Ticket!.CompanyName),

                ("status", "asc") =>
                    query.OrderBy(j => j.Status),

                ("status", "desc") =>
                    query.OrderByDescending(j => j.Status),

                ("technician", "asc") =>
                    query.OrderBy(j => j.AssignedTechnician!.FirstName),

                ("technician", "desc") =>
                    query.OrderByDescending(j => j.AssignedTechnician!.FirstName),

                ("datecreated", "asc") =>
                    query.OrderBy(j => j.DateCreated),

                _ =>
                    query.OrderByDescending(j => j.DateCreated)
            };

            //--------------------------------------------------
            // Return DTO List
            //--------------------------------------------------
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new JobCardListDto
                {
                    JobCardId = j.JobCardId,

                    JobNumber = j.JobNumber,

                    TicketId = j.TicketId,

                    CustomerName =
                        j.Ticket != null
                            ? j.Ticket.CustomerName
                            : "",

                    CompanyName =
                        j.Ticket != null
                            ? j.Ticket.CompanyName
                            : "",

                    Subject =
                        j.Ticket != null
                            ? j.Ticket.Subject
                            : "",

                    AssignedTechnicianId = j.AssignedTechnicianId,

                    AssignedTechnicianName =
                        j.AssignedTechnician != null
                            ? j.AssignedTechnician.FirstName + " " +
                              j.AssignedTechnician.LastName
                            : "Not Assigned",

                    Status = j.Status,

                    DateCreated = j.DateCreated,

                    DateCompleted = j.DateCompleted
                })
                .ToListAsync();

            return new PagedResultDto<JobCardListDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        //--------------------------------------------------
        // Get By Id
        //--------------------------------------------------

        public async Task<JobCard?> GetByIdAsync(int id)
        {
            return await _context.JobCards
                .FirstOrDefaultAsync(j => j.JobCardId == id);
        }

        //--------------------------------------------------
        // Get By Ticket
        //--------------------------------------------------

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

                    AssignedTechnicianId = j.AssignedTechnicianId,

                    AssignedTechnician =
                        j.AssignedTechnician != null
                            ? j.AssignedTechnician.FirstName + " " +
                              j.AssignedTechnician.LastName
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

        //--------------------------------------------------
        // Save
        //--------------------------------------------------

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        //--------------------------------------------------
        // Latest Job Card
        //--------------------------------------------------

        public async Task<JobCard?> GetLatestJobCardAsync()
        {
            return await _context.JobCards
                .OrderByDescending(j => j.JobCardId)
                .FirstOrDefaultAsync();
        }

        //--------------------------------------------------
        // Job Number Exists
        //--------------------------------------------------

        public async Task<bool> JobNumberExistsAsync(string jobNumber)
        {
            return await _context.JobCards
                .AnyAsync(j => j.JobNumber == jobNumber);
        }

        //--------------------------------------------------
        // Labour
        //--------------------------------------------------

        public async Task AddLabourEntryAsync(JobCardLabour labour)
        {
            await _context.JobCardLabours.AddAsync(labour);
        }

        public async Task<List<JobCardLabour>> GetLabourEntriesAsync(int jobCardId)
        {
            return await _context.JobCardLabours
                .Include(l => l.Technician)
                .Where(l => l.JobCardId == jobCardId)
                .OrderBy(l => l.DateWorked)
                .ToListAsync();
        }


        //--------------------------------------------------
        // Parts
        //--------------------------------------------------

        public async Task AddPartAsync(JobCardPart part)
        {
            await _context.JobCardParts.AddAsync(part);
        }

        //--------------------------------------------------

        public async Task<List<JobCardPart>> GetPartsAsync(int jobCardId)
        {
            return await _context.JobCardParts
                .Where(p => p.JobCardId == jobCardId)
                .OrderBy(p => p.PartName)
                .ToListAsync();
        }

        //--------------------------------------------------

        public async Task<JobCardPart?> GetPartByIdAsync(int partId)
        {
            return await _context.JobCardParts
                .FirstOrDefaultAsync(p => p.PartId == partId);
        }

        //--------------------------------------------------

        public async Task DeletePartAsync(JobCardPart part)
        {
            _context.JobCardParts.Remove(part);

            await Task.CompletedTask;
        }
    }
}
using IThelpdesk.DTOs.Common;
using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;

namespace IThelpdesk.Services
{
    public class JobCardService : IJobCardService
    {
        private readonly IJobCardRepository _jobCardRepository;
        private readonly ITicketRepository _ticketRepository;

        public JobCardService(
            IJobCardRepository jobCardRepository,
            ITicketRepository ticketRepository)
        {
            _jobCardRepository = jobCardRepository;
            _ticketRepository = ticketRepository;
        }

        //---------------------------------------------------
        // Get All Job Cards
        //---------------------------------------------------

        public async Task<IEnumerable<JobCard>> GetAllAsync()
        {
            return await _jobCardRepository.GetAllAsync();
        }

        //---------------------------------------------------
        // Get Job Card List
        //---------------------------------------------------

        public async Task<PagedResultDto<JobCardListDto>> GetJobCardListAsync(
      int userId,
      string role,
      bool mine,
      string? status,
      int? assignedTo,
      string? search,
      string? sortBy,
      string? sortDirection,
      int pageNumber,
      int pageSize)
        {
            int? technicianId = null;

            //---------------------------------------------------
            // Administrators
            //---------------------------------------------------

            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                if (mine)
                {
                    technicianId = userId;
                }
            }

            //---------------------------------------------------
            // Technicians
            //---------------------------------------------------

            else
            {
                technicianId = userId;
            }

            return await _jobCardRepository.GetJobCardListAsync(
            technicianId,
            status,
            assignedTo,
            search,
            sortBy,
            sortDirection,
            pageNumber,
            pageSize);

        }

        //---------------------------------------------------
        // Get By Id
        //---------------------------------------------------

        public async Task<JobCard?> GetByIdAsync(int id)
        {
            return await _jobCardRepository.GetByIdAsync(id);
        }

        //---------------------------------------------------
        // Get Details
        //---------------------------------------------------

        public async Task<JobCardDetailsDto?> GetDetailsAsync(int id)
        {
            return await _jobCardRepository.GetDetailsAsync(id);
        }

        //---------------------------------------------------
        // Create From Ticket
        //---------------------------------------------------

        public async Task<JobCard> CreateFromTicketAsync(int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found.");

            if (!string.Equals(ticket.Status, "Resolved", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("A Job Card can only be created after the ticket has been resolved.");
            }

            var existing = await _jobCardRepository.GetByTicketIdAsync(ticketId);

            if (existing != null)
            {
                throw new Exception($"This ticket already has Job Card {existing.JobNumber}");
            }

            //---------------------------------------------------
            // Generate Job Number
            //---------------------------------------------------

            var latest = await _jobCardRepository.GetLatestJobCardAsync();

            int nextNumber = 1;

            if (latest != null)
            {
                var parts = latest.JobNumber.Split('-');

                if (parts.Length == 3 &&
                    int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            string jobNumber = $"JC-{DateTime.Now.Year}-{nextNumber:D6}";

            //---------------------------------------------------
            // Create Job Card
            //---------------------------------------------------

            var jobCard = new JobCard
            {
                TicketId = ticket.TicketId,
                AssignedTechnicianId = ticket.AssignedToUserId,
                JobNumber = jobNumber,
                Status = "Open",
                DateCreated = DateTime.UtcNow,
                FaultReported = ticket.Description,
                FaultFound = "",
                WorkPerformed = "",
                CompletionNotes = "",
                CustomerName = ticket.CustomerName,
                CustomerSignature = "",
                SignedDate = null
            };

            await _jobCardRepository.AddAsync(jobCard);
            await _jobCardRepository.SaveChangesAsync();

            return jobCard;
        }

        //---------------------------------------------------
        // Update Job Card
        //---------------------------------------------------

        public async Task UpdateJobCardAsync(
            int id,
            UpdateJobCardDto dto)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            jobCard.Status = dto.Status;
            jobCard.FaultFound = dto.FaultFound;
            jobCard.WorkPerformed = dto.WorkPerformed;
            jobCard.CompletionNotes = dto.CompletionNotes;
            jobCard.CustomerSignature = dto.CustomerSignature;

            jobCard.DateCompleted =
                dto.Status == "Completed"
                ? DateTime.UtcNow
                : null;

            await _jobCardRepository.UpdateAsync(jobCard);
            await _jobCardRepository.SaveChangesAsync();
        }

        //---------------------------------------------------
        // Complete Job Card
        //---------------------------------------------------

        public async Task CompleteJobCardAsync(int id)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            jobCard.Status = "Completed";
            jobCard.DateCompleted = DateTime.UtcNow;

            await _jobCardRepository.UpdateAsync(jobCard);
            await _jobCardRepository.SaveChangesAsync();
        }

        //---------------------------------------------------
        // Update
        //---------------------------------------------------

        public async Task UpdateAsync(JobCard jobCard)
        {
            await _jobCardRepository.UpdateAsync(jobCard);
            await _jobCardRepository.SaveChangesAsync();
        }

        //---------------------------------------------------
        // Delete
        //---------------------------------------------------

        public async Task DeleteAsync(int id)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
                return;

            await _jobCardRepository.DeleteAsync(jobCard);
            await _jobCardRepository.SaveChangesAsync();
        }

        //---------------------------------------------------
        // Add Labour
        //---------------------------------------------------

        public async Task AddLabourEntryAsync(
            int jobCardId,
            AddLabourEntryDto dto)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            if (jobCard.AssignedTechnicianId == null)
                throw new Exception("No technician assigned.");

            var labour = new JobCardLabour
            {
                JobCardId = jobCardId,
                TechnicianId = jobCard.AssignedTechnicianId.Value,
                HoursWorked = dto.HoursWorked,
                WorkPerformed = dto.WorkPerformed,
                DateWorked = DateTime.UtcNow
            };

            await _jobCardRepository.AddLabourEntryAsync(labour);
            await _jobCardRepository.SaveChangesAsync();
        }

        //---------------------------------------------------
        // Get Labour
        //---------------------------------------------------

        public async Task<List<JobCardLabour>> GetLabourEntriesAsync(int jobCardId)
        {
            return await _jobCardRepository.GetLabourEntriesAsync(jobCardId);
        }
    }
}
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
        // Get All
        //---------------------------------------------------

        public async Task<IEnumerable<JobCard>> GetAllAsync()
        {
            return await _jobCardRepository.GetAllAsync();
        }

        //---------------------------------------------------
        // Get Job Card List
        //---------------------------------------------------

        public async Task<IEnumerable<JobCardListDto>> GetJobCardListAsync(
            int userId,
            string role)
        {
            //---------------------------------------------------
            // Administrator
            //---------------------------------------------------

            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return await _jobCardRepository.GetJobCardListAsync();
            }

            //---------------------------------------------------
            // Technician
            //---------------------------------------------------

            return await _jobCardRepository.GetJobCardListByTechnicianAsync(userId);
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

            if (!string.Equals(
                    ticket.Status,
                    "Resolved",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("A Job Card can only be created after the ticket has been resolved.");
            }

            var existing = await _jobCardRepository.GetByTicketIdAsync(ticketId);

            if (existing != null)
                throw new Exception($"This ticket already has Job Card {existing.JobNumber}");

            //---------------------------------------------------
            // Generate Next Job Number
            //---------------------------------------------------

            var latestJobCard = await _jobCardRepository.GetLatestJobCardAsync();

            int nextNumber = 1;

            if (latestJobCard != null)
            {
                string[] parts = latestJobCard.JobNumber.Split('-');

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
        // Add Labour Entry
        //---------------------------------------------------

        public async Task AddLabourEntryAsync(
            int jobCardId,
            AddLabourEntryDto dto)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            if (jobCard.AssignedTechnicianId == null)
            {
                throw new Exception("No technician has been assigned to this Job Card.");
            }

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
        // Get Labour Entries
        //---------------------------------------------------

        public async Task<List<JobCardLabour>> GetLabourEntriesAsync(int jobCardId)
        {
            return await _jobCardRepository.GetLabourEntriesAsync(jobCardId);
        }

        //---------------------------------------------------
        // Update Job Card
        //---------------------------------------------------

        public async Task UpdateJobCardAsync(int id, UpdateJobCardDto dto)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            jobCard.Status = dto.Status;
            jobCard.FaultFound = dto.FaultFound;
            jobCard.WorkPerformed = dto.WorkPerformed;
            jobCard.CompletionNotes = dto.CompletionNotes;
            jobCard.CustomerSignature = dto.CustomerSignature;

            if (string.Equals(dto.Status, "Completed", StringComparison.OrdinalIgnoreCase))
            {
                jobCard.DateCompleted = DateTime.UtcNow;
            }
            else
            {
                jobCard.DateCompleted = null;
            }

            await _jobCardRepository.UpdateAsync(jobCard);

            await _jobCardRepository.SaveChangesAsync();
        }

        //---------------------------------------------------
        // Complete Job Card
        //---------------------------------------------------
        //
        // Marks a Job Card as completed.
        //
        //---------------------------------------------------

        public async Task CompleteJobCardAsync(int id)
        {
            //---------------------------------------------------
            // Retrieve Job Card
            //---------------------------------------------------

            var jobCard = await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
            {
                throw new Exception("Job Card not found.");
            }

            //---------------------------------------------------
            // Prevent duplicate completion
            //---------------------------------------------------

            if (string.Equals(
                    jobCard.Status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("This Job Card has already been completed.");
            }

            //---------------------------------------------------
            // Mark as completed
            //---------------------------------------------------

            jobCard.Status = "Completed";
            jobCard.DateCompleted = DateTime.UtcNow;

            //---------------------------------------------------
            // Save
            //---------------------------------------------------

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
    }
}
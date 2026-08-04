using IThelpdesk.DTOs.Common;
using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;
using IThelpdesk.Enums;
using System.Linq;

namespace IThelpdesk.Services
{
    public class JobCardService : IJobCardService
    {
        private readonly IJobCardRepository _jobCardRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IJobCardAuditService _auditService;

        public JobCardService(
            IJobCardRepository jobCardRepository,
            ITicketRepository ticketRepository,
            IJobCardAuditService auditService)
        {
            _jobCardRepository = jobCardRepository;
            _ticketRepository = ticketRepository;
            _auditService = auditService;
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

        public async Task<JobCard> CreateFromTicketAsync(
         int ticketId,
         int currentUserId)
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
                return existing;
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

            await _auditService.LogAsync(
             jobCard.JobCardId,
             currentUserId,
             JobCardAuditAction.JobCardCreated,
             $"Job Card {jobCard.JobNumber} created from Ticket #{ticket.TicketId}"
         );

            return jobCard;
        }

        //---------------------------------------------------
        // Update Job Card
        //---------------------------------------------------

        public async Task UpdateJobCardAsync(
            int id,
            UpdateJobCardDto dto,
            int currentUserId) // ✅ method signature already updated
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            //---------------------------------------------------
            // Store Original Values
            //---------------------------------------------------

            var oldStatus = jobCard.Status;
            var oldFaultFound = jobCard.FaultFound;
            var oldWorkPerformed = jobCard.WorkPerformed;
            var oldCompletionNotes = jobCard.CompletionNotes;
            var oldCustomerSignature = jobCard.CustomerSignature;

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

            //---------------------------------------------------
            // Audit Changes
            //---------------------------------------------------

            // ❌ Removed: int userId = jobCard.AssignedTechnicianId ?? 0;

            // Status Changed (treat Completed as JobCardCompleted)
            if (oldStatus != dto.Status)
            {
                var action = dto.Status == "Completed"
                    ? JobCardAuditAction.JobCardCompleted
                    : JobCardAuditAction.StatusChanged;

                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    currentUserId, // ✅ replaced userId
                    action,
                    $"Status changed from '{oldStatus}' to '{dto.Status}'",
                    oldStatus,
                    dto.Status);
            }

            // Fault Found Updated
            if (oldFaultFound != dto.FaultFound)
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    currentUserId, // ✅ replaced userId
                    JobCardAuditAction.FaultFoundUpdated,
                    "Fault Found updated",
                    oldFaultFound,
                    dto.FaultFound);
            }

            // Work Performed Updated
            if (oldWorkPerformed != dto.WorkPerformed)
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    currentUserId, // ✅ replaced userId
                    JobCardAuditAction.WorkPerformedUpdated,
                    "Work Performed updated",
                    oldWorkPerformed,
                    dto.WorkPerformed);
            }

            // Completion Notes Updated
            if (oldCompletionNotes != dto.CompletionNotes)
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    currentUserId, // ✅ replaced userId
                    JobCardAuditAction.CompletionNotesUpdated,
                    "Completion Notes updated",
                    oldCompletionNotes,
                    dto.CompletionNotes);
            }

            // Customer Signature Added
            if (oldCustomerSignature != dto.CustomerSignature &&
                !string.IsNullOrWhiteSpace(dto.CustomerSignature))
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    currentUserId, // ✅ replaced userId
                    JobCardAuditAction.CustomerSignatureAdded,
                    "Customer signature captured");
            }
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

            // NOTE:
            // Completion audit is handled in UpdateJobCardAsync to avoid duplicate completion entries.
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

        public async Task DeleteAsync(
        int id,
        int currentUserId)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
                return;

            await _auditService.LogAsync(
            jobCard.JobCardId,
            currentUserId,
            JobCardAuditAction.JobCardDeleted,
            $"Job Card {jobCard.JobNumber} deleted");

            await _jobCardRepository.DeleteAsync(jobCard);
            await _jobCardRepository.SaveChangesAsync();
        }
        //---------------------------------------------------
        // Add Labour
        //---------------------------------------------------
        public async Task AddLabourEntryAsync(
            int jobCardId,
            AddLabourEntryDto dto,
            int currentUserId)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            if (jobCard.Status == "Completed")
                throw new Exception("Completed Job Cards cannot be modified.");

            if (jobCard.AssignedTechnicianId == null)
                throw new Exception("No technician assigned.");

            if (dto.HoursWorked <= 0)
                throw new Exception("Hours worked must be greater than zero.");

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

                    await _auditService.LogAsync(
            jobCard.JobCardId,
            currentUserId,
            JobCardAuditAction.LabourAdded,
            $"Added {dto.HoursWorked} hours of labour.");
        }


        //---------------------------------------------------
        // Get Labour
        //---------------------------------------------------
        public async Task<List<JobCardLabour>> GetLabourEntriesAsync(int jobCardId)
        {
            return await _jobCardRepository.GetLabourEntriesAsync(jobCardId);
        }


        //---------------------------------------------------
        // Add Part
        //---------------------------------------------------
        public async Task AddPartAsync(
            int jobCardId,
            AddPartDto dto,
            int currentUserId)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            if (jobCard.Status == "Completed")
                throw new Exception("Completed Job Cards cannot be modified.");

            if (dto.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

            if (string.IsNullOrWhiteSpace(dto.PartName))
                throw new Exception("Part name is required.");

            var part = new JobCardPart
            {
                JobCardId = jobCardId,
                PartName = dto.PartName.Trim(),
                Quantity = dto.Quantity
            };

            await _jobCardRepository.AddPartAsync(part);
            await _jobCardRepository.SaveChangesAsync();

            await _auditService.LogAsync(
              jobCard.JobCardId,
              currentUserId,
              JobCardAuditAction.PartAdded,
              $"Added part '{dto.PartName.Trim()}' x{dto.Quantity}");
        }



        //---------------------------------------------------
        // Get Parts
        //---------------------------------------------------

        public async Task<List<JobCardPartDto>> GetPartsAsync(int jobCardId)
        {
            var parts = await _jobCardRepository.GetPartsAsync(jobCardId);

            return parts.Select(p => new JobCardPartDto
            {
                PartId = p.PartId,
                PartName = p.PartName,
                Quantity = p.Quantity
            }).ToList();
        }

        //---------------------------------------------------
        // Delete Part
        //---------------------------------------------------
        public async Task DeletePartAsync(
        int partId,
        int currentUserId)
        {
            var part = await _jobCardRepository.GetPartByIdAsync(partId);

            if (part == null)
                throw new Exception("Part not found.");

            var jobCard = await _jobCardRepository.GetByIdAsync(part.JobCardId);
            
            if (jobCard != null && jobCard.Status == "Completed")
                throw new Exception("Completed Job Cards cannot be modified.");

            await _auditService.LogAsync(
            part.JobCardId,
            currentUserId,
            JobCardAuditAction.PartDeleted,
            $"Deleted part '{part.PartName}' x{part.Quantity}");

            await _jobCardRepository.DeletePartAsync(part);
            await _jobCardRepository.SaveChangesAsync();
        }

    }
}

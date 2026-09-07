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
        private readonly IUserRepository _userRepository;    
        private readonly INotificationService _notificationService;

        public JobCardService(
    IJobCardRepository jobCardRepository,
    ITicketRepository ticketRepository,
    IJobCardAuditService auditService,
    IUserRepository userRepository,
    INotificationService notificationService)
        {
            _jobCardRepository = jobCardRepository;
            _ticketRepository = ticketRepository;
            _auditService = auditService;
            _userRepository = userRepository;
            _notificationService = notificationService;
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

            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                if (mine)
                {
                    technicianId = userId;
                }
            }
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
    int performedByUserId)
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

            var latest = await _jobCardRepository.GetLatestJobCardAsync();

            int nextNumber = 1;

            if (latest != null && !string.IsNullOrWhiteSpace(latest.JobNumber))
            {
                var parts = latest.JobNumber.Split('-');

                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            string jobNumber = $"JC-{DateTime.Now.Year}-{nextNumber:D6}";

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

            try
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    performedByUserId,
                    JobCardAuditAction.JobCardCreated,
                    $"Job Card {jobCard.JobNumber} created from Ticket #{ticket.TicketId}"
                );
            }
            catch (Exception auditException)
            {
                Console.WriteLine(
                    $"WARNING: Job Card {jobCard.JobCardId} was created, but audit logging failed."
                );

                Console.WriteLine(auditException);
            }

            //--------------------------------------------------
            // Notify the assigned technician that the Job Card
            // was created.
            //--------------------------------------------------

            if (jobCard.AssignedTechnicianId.HasValue)
            {
                await _notificationService.CreateAsync(
                    jobCard.AssignedTechnicianId.Value,
                    "New Job Card Assigned",
                    $"Job Card {jobCard.JobNumber} has been created and assigned to you.");
            }

            return jobCard;
        }

        //---------------------------------------------------
        // Update Job Card
        //---------------------------------------------------

        public async Task UpdateJobCardAsync(
    int id,
    UpdateJobCardDto dto,
    int performedByUserId)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

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

            jobCard.DateCompleted = dto.Status == "Completed" ? DateTime.UtcNow : null;

            await _jobCardRepository.UpdateAsync(jobCard);
            await _jobCardRepository.SaveChangesAsync();

            int userId = performedByUserId;

            if (oldStatus != dto.Status)
            {
                var action = dto.Status == "Completed"
                    ? JobCardAuditAction.JobCardCompleted
                    : JobCardAuditAction.StatusChanged;

                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    userId,
                    action,
                    $"Status changed from '{oldStatus}' to '{dto.Status}'",
                    oldStatus,
                    dto.Status);
            }

            if (oldFaultFound != dto.FaultFound)
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    userId,
                    JobCardAuditAction.FaultFoundUpdated,
                    "Fault Found updated",
                    oldFaultFound,
                    dto.FaultFound);
            }

            if (oldWorkPerformed != dto.WorkPerformed)
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    userId,
                    JobCardAuditAction.WorkPerformedUpdated,
                    "Work Performed updated",
                    oldWorkPerformed,
                    dto.WorkPerformed);
            }

            if (oldCompletionNotes != dto.CompletionNotes)
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    userId,
                    JobCardAuditAction.CompletionNotesUpdated,
                    "Completion Notes updated",
                    oldCompletionNotes,
                    dto.CompletionNotes);
            }

            if (oldCustomerSignature != dto.CustomerSignature && !string.IsNullOrWhiteSpace(dto.CustomerSignature))
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    userId,
                    JobCardAuditAction.CustomerSignatureAdded,
                    "Customer signature captured");
            }
        }

        //---------------------------------------------------
        // Complete Job Card
        //---------------------------------------------------

        public async Task CompleteJobCardAsync(
          int id,
          int performedByUserId)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            var oldStatus = jobCard.Status;

            jobCard.Status = "Completed";
            jobCard.DateCompleted = DateTime.UtcNow;

            await _jobCardRepository.UpdateAsync(jobCard);
            await _jobCardRepository.SaveChangesAsync();

            // Record the actual user who performed the completion.
            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.JobCardCompleted,
                $"Job Card {jobCard.JobNumber} completed.",
                oldStatus,
                "Completed");

            //--------------------------------------------------
            // Notify Admins that the Job Card was completed
            //--------------------------------------------------

            var admins = await _userRepository.GetAdminsAsync();

            foreach (var admin in admins)
            {
                await _notificationService.CreateAsync(
                    admin.UserId,
                    "Job Card Completed",
                    $"Job Card {jobCard.JobNumber} has been completed.");
            }
        }
        public async Task UpdateAsync(JobCard jobCard)
        {
            await _jobCardRepository.UpdateAsync(jobCard);
            await _jobCardRepository.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
                return;

            await _auditService.LogAsync(
                jobCard.JobCardId,
                jobCard.AssignedTechnicianId ?? 0,
                JobCardAuditAction.JobCardDeleted,
                $"Job Card {jobCard.JobNumber} deleted");

            await _jobCardRepository.DeleteAsync(jobCard);
            await _jobCardRepository.SaveChangesAsync();
        }

        //---------------------------------------------------
        // Add / Get Labour
        //---------------------------------------------------

        public async Task AddLabourEntryAsync(int jobCardId, AddLabourEntryDto dto, int performedByUserId)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            if (jobCard.AssignedTechnicianId == null)
                throw new Exception("No technician assigned.");

            var labour = new JobCardLabour
            {
                JobCardId = jobCardId,
                // Use the logged-in user's id (performedByUserId) as the TechnicianId
                TechnicianId = performedByUserId,
                HoursWorked = dto.HoursWorked,
                WorkPerformed = dto.WorkPerformed,
                DateWorked = DateTime.UtcNow
            };

            await _jobCardRepository.AddLabourEntryAsync(labour);
            await _jobCardRepository.SaveChangesAsync();

            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.LabourAdded,
                $"Added {dto.HoursWorked} hours of labour.");
        }

        public async Task<List<JobCardLabourEntryDto>> GetLabourEntriesAsync(int jobCardId)
        {
            var labourEntries = await _jobCardRepository.GetLabourEntriesAsync(jobCardId);
            var result = new List<JobCardLabourEntryDto>();

            foreach (var entry in labourEntries)
            {
                var user = await _userRepository.GetUserEntityByIdAsync(entry.TechnicianId);

                string technicianName = user != null
                    ? $"{user.FirstName} {user.LastName}"
                    : $"Technician {entry.TechnicianId}";

                result.Add(new JobCardLabourEntryDto
                {
                    LabourId = entry.LabourId,
                    JobCardId = entry.JobCardId,
                    TechnicianId = entry.TechnicianId,
                    TechnicianName = technicianName,
                    HoursWorked = entry.HoursWorked,
                    WorkPerformed = entry.WorkPerformed,
                    DateWorked = entry.DateWorked
                });
            }

            return result;
        }

        //---------------------------------------------------
        // Update / Delete Labour
        //---------------------------------------------------

        public async Task UpdateLabourEntryAsync(
            int jobCardId,
            int labourId,
            UpdateLabourEntryDto dto,
            int performedByUserId,
            string role)
        {
            var labour = await _jobCardRepository.GetLabourByIdAsync(labourId);
            if (labour == null)
                throw new Exception("Labour entry not found.");

            if (labour.JobCardId != jobCardId)
                throw new Exception("Labour entry does not belong to this Job Card.");

            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                throw new Exception("Job Card not found.");

            if (string.Equals(role, "Technician", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(jobCard.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Technicians cannot modify labour on a completed Job Card.");

                if (jobCard.AssignedTechnicianId != performedByUserId)
                    throw new Exception("You can only modify labour on Job Cards assigned to you.");
            }

            if (dto.HoursWorked <= 0)
                throw new Exception("Hours worked must be greater than zero.");

            var oldHours = labour.HoursWorked;
            var oldWork = labour.WorkPerformed;

            labour.HoursWorked = dto.HoursWorked;
            labour.WorkPerformed = dto.WorkPerformed;

            await _jobCardRepository.UpdateLabourEntryAsync(labour);
            await _jobCardRepository.SaveChangesAsync();

            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.LabourUpdated,
                $"Updated labour entry from {oldHours}h to {dto.HoursWorked}h.");
        }

        public async Task DeleteLabourEntryAsync(
            int labourId,
            int performedByUserId,
            string role)
        {
            var labour = await _jobCardRepository.GetLabourByIdAsync(labourId);
            if (labour == null)
                throw new Exception("Labour entry not found.");

            var jobCardId = labour.JobCardId;

            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                throw new Exception("Job Card not found.");

            if (string.Equals(role, "Technician", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(jobCard.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Technicians cannot delete labour from a completed Job Card.");

                if (jobCard.AssignedTechnicianId != performedByUserId)
                    throw new Exception("You can only delete labour on Job Cards assigned to you.");
            }

            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.LabourDeleted,
                $"Deleted labour entry of {labour.HoursWorked} hours.");

            await _jobCardRepository.DeleteLabourEntryAsync(labour);
            await _jobCardRepository.SaveChangesAsync();
        }



        //---------------------------------------------------
        // Parts: Add / Get / Update / Delete
        //---------------------------------------------------

        public async Task AddPartAsync(
            int jobCardId,
            AddPartDto dto,
            int performedByUserId, // Match interface (int)
            string role)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                throw new Exception("Job Card not found.");

            if (string.Equals(role, "Technician", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(jobCard.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Technicians cannot add parts to a completed Job Card.");

                if (jobCard.AssignedTechnicianId != performedByUserId)
                    throw new Exception("You can only add parts to Job Cards assigned to you.");
            }

            if (dto.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

            if (string.IsNullOrWhiteSpace(dto.PartName))
                throw new Exception("Part name is required.");

            var part = new JobCardPart
            {
                JobCardId = jobCardId,
                PartName = dto.PartName.Trim(),
                Quantity = dto.Quantity,
                CreatedByUserId = performedByUserId.ToString(), // Convert int to string for Model
                DateAdded = DateTime.UtcNow
            };

            await _jobCardRepository.AddPartAsync(part);
            await _jobCardRepository.SaveChangesAsync();

            await _auditService.LogAsync(
      jobCard.JobCardId,
      performedByUserId,
      JobCardAuditAction.PartAdded,
      $"Added part '{part.PartName}' x{part.Quantity}");

            //--------------------------------------------------
            // Notify Admins that a part was added
            //--------------------------------------------------

            var admins = await _userRepository.GetAdminsAsync();

            foreach (var admin in admins)
            {
                await _notificationService.CreateAsync(
                    admin.UserId,
                    "Part Added",
                    $"Part '{part.PartName}' x{part.Quantity} was added to Job Card {jobCard.JobNumber}.");
            }
        }

        public async Task<List<JobCardPartDto>> GetPartsAsync(int jobCardId)
        {
            var parts = await _jobCardRepository.GetPartsAsync(jobCardId);
            var result = new List<JobCardPartDto>();

            foreach (var p in parts)
            {
                // Parse string CreatedByUserId to int for repository lookup (CS1503 resolved)
                int.TryParse(p.CreatedByUserId, out int createdById);

                var user = createdById > 0
                    ? await _userRepository.GetUserEntityByIdAsync(createdById)
                    : null;

                string addedBy = user != null
                    ? $"{user.FirstName} {user.LastName}"
                    : $"User {p.CreatedByUserId}";

                result.Add(new JobCardPartDto
                {
                    PartId = p.PartId,
                    JobCardId = p.JobCardId,
                    PartName = p.PartName,
                    Quantity = p.Quantity,
                    AddedByName = addedBy,
                    AddedByUserId = createdById, // Assign parsed int (CS0029 resolved)
                    DateAdded = p.DateAdded
                });
            }

            return result;
        }

        public async Task UpdatePartAsync(
            int jobCardId,
            int partId,
            UpdatePartDto dto,
            int performedByUserId, // Match interface (int)
            string role)
        {
            var part = await _jobCardRepository.GetPartByIdAsync(partId);
            if (part == null)
                throw new Exception("Part not found.");

            if (part.JobCardId != jobCardId)
                throw new Exception("Part does not belong to this Job Card.");

            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                throw new Exception("Job Card not found.");

            if (string.Equals(role, "Technician", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(jobCard.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Technicians cannot modify parts on a completed Job Card.");

                if (jobCard.AssignedTechnicianId != performedByUserId)
                    throw new Exception("You can only modify parts on Job Cards assigned to you.");
            }

            if (string.IsNullOrWhiteSpace(dto.PartName))
                throw new Exception("Part name is required.");

            if (dto.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

            var oldPartName = part.PartName;
            var oldQuantity = part.Quantity;

            part.PartName = dto.PartName.Trim();
            part.Quantity = dto.Quantity;

            await _jobCardRepository.UpdatePartAsync(part);
            await _jobCardRepository.SaveChangesAsync();

            if (oldPartName != dto.PartName)
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    performedByUserId,
                    JobCardAuditAction.PartNameUpdated,
                    "Part name updated",
                    oldPartName,
                    dto.PartName);
            }

            if (oldQuantity != dto.Quantity)
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    performedByUserId,
                    JobCardAuditAction.PartQuantityUpdated,
                    "Part quantity updated",
                    oldQuantity.ToString(),
                    dto.Quantity.ToString());
            }
        }

        public async Task DeletePartAsync(
            int partId,
            int performedByUserId, // Match interface (int)
            string role)
        {
            var part = await _jobCardRepository.GetPartByIdAsync(partId);
            if (part == null)
                throw new Exception("Part not found.");

            var jobCard = await _jobCardRepository.GetByIdAsync(part.JobCardId);
            if (jobCard == null)
                throw new Exception("Job Card not found.");

            if (string.Equals(role, "Technician", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(jobCard.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Technicians cannot delete parts from a completed Job Card.");

                if (jobCard.AssignedTechnicianId != performedByUserId)
                    throw new Exception("You can only delete parts on Job Cards assigned to you.");
            }

            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.PartDeleted,
                $"Deleted part '{part.PartName}' x{part.Quantity}");

            await _jobCardRepository.DeletePartAsync(part);
            await _jobCardRepository.SaveChangesAsync();
        }
        public async Task<JobCard?> GetByTicketIdAsync(int ticketId)
        {
            return await _jobCardRepository.GetByTicketIdAsync(ticketId);
        }
    }
}
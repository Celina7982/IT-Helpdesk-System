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

            if (string.Equals(
                role,
                "Admin",
                StringComparison.OrdinalIgnoreCase))
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
    int performedByUserId)
        {
            //---------------------------------------------------
            // Find Ticket
            //---------------------------------------------------

            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found.");

            //---------------------------------------------------
            // Ticket must be resolved
            //---------------------------------------------------

            if (!string.Equals(
                ticket.Status,
                "Resolved",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "A Job Card can only be created after the ticket has been resolved.");
            }

            //---------------------------------------------------
            // Check whether Job Card already exists
            //---------------------------------------------------

            var existing =
                await _jobCardRepository.GetByTicketIdAsync(ticketId);

            if (existing != null)
            {
                return existing;
            }

            //---------------------------------------------------
            // Generate Job Number
            //---------------------------------------------------

            var latest =
                await _jobCardRepository.GetLatestJobCardAsync();

            int nextNumber = 1;

            if (latest != null &&
                !string.IsNullOrWhiteSpace(latest.JobNumber))
            {
                var parts = latest.JobNumber.Split('-');

                if (parts.Length == 3 &&
                    int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            string jobNumber =
                $"JC-{DateTime.Now.Year}-{nextNumber:D6}";

            //---------------------------------------------------
            // Create Job Card
            //---------------------------------------------------

            var jobCard = new JobCard
            {
                TicketId = ticket.TicketId,

                AssignedTechnicianId =
                    ticket.AssignedToUserId,

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

            //---------------------------------------------------
            // Save Job Card
            //---------------------------------------------------

            await _jobCardRepository.AddAsync(jobCard);

            await _jobCardRepository.SaveChangesAsync();

           
            //---------------------------------------------------
            // Return Job Card
            //---------------------------------------------------

            return jobCard;
        }

        //---------------------------------------------------
        // Update Job Card
        //---------------------------------------------------

        public async Task UpdateJobCardAsync(
     int id,
     UpdateJobCardDto dto,
     int performedByUserId,
     string role)
        {
            var jobCard =
                await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            //---------------------------------------------------
            // Technician permissions
            //---------------------------------------------------

            if (string.Equals(
                role,
                "Technician",
                StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(
                    jobCard.Status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception(
                        "Technicians cannot modify a completed Job Card.");
                }

                if (jobCard.AssignedTechnicianId != performedByUserId)
                {
                    throw new Exception(
                        "You can only modify Job Cards assigned to you.");
                }
            }

            //---------------------------------------------------
            // Store Original Values
            //---------------------------------------------------

            var oldStatus = jobCard.Status;
            var oldFaultFound = jobCard.FaultFound;
            var oldWorkPerformed = jobCard.WorkPerformed;
            var oldCompletionNotes = jobCard.CompletionNotes;
            var oldCustomerSignature = jobCard.CustomerSignature;

            //---------------------------------------------------
            // Update Values
            //---------------------------------------------------

            jobCard.Status = dto.Status;
            jobCard.FaultFound = dto.FaultFound;
            jobCard.WorkPerformed = dto.WorkPerformed;
            jobCard.CompletionNotes = dto.CompletionNotes;
            jobCard.CustomerSignature = dto.CustomerSignature;

            jobCard.DateCompleted =
                dto.Status == "Completed"
                    ? DateTime.UtcNow
                    : null;

            //---------------------------------------------------
            // Save
            //---------------------------------------------------

            await _jobCardRepository.UpdateAsync(jobCard);

            await _jobCardRepository.SaveChangesAsync();

            //---------------------------------------------------
            // Audit Changes
            //---------------------------------------------------

            if (oldStatus != dto.Status)
            {
                var action =
                    dto.Status == "Completed"
                        ? JobCardAuditAction.JobCardCompleted
                        : JobCardAuditAction.StatusChanged;

                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    performedByUserId,
                    action,
                    $"Status changed from '{oldStatus}' to '{dto.Status}'",
                    oldStatus,
                    dto.Status);
            }

            //---------------------------------------------------
            // Fault Found Updated
            //---------------------------------------------------

            if (oldFaultFound != dto.FaultFound)
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    performedByUserId,
                    JobCardAuditAction.FaultFoundUpdated,
                    "Fault Found updated",
                    oldFaultFound,
                    dto.FaultFound);
            }

            //---------------------------------------------------
            // Work Performed Updated
            //---------------------------------------------------

            if (oldWorkPerformed != dto.WorkPerformed)
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    performedByUserId,
                    JobCardAuditAction.WorkPerformedUpdated,
                    "Work Performed updated",
                    oldWorkPerformed,
                    dto.WorkPerformed);
            }

            //---------------------------------------------------
            // Completion Notes Updated
            //---------------------------------------------------

            if (oldCompletionNotes != dto.CompletionNotes)
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    performedByUserId,
                    JobCardAuditAction.CompletionNotesUpdated,
                    "Completion Notes updated",
                    oldCompletionNotes,
                    dto.CompletionNotes);
            }

            //---------------------------------------------------
            // Customer Signature
            //---------------------------------------------------
            // Leave this temporarily.
            // We will remove CustomerSignature properly after
            // the audit-user changes are working.
            //---------------------------------------------------

            if (oldCustomerSignature != dto.CustomerSignature &&
                !string.IsNullOrWhiteSpace(dto.CustomerSignature))
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    performedByUserId,
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
            var jobCard =
                await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            var oldStatus = jobCard.Status;

            jobCard.Status = "Completed";

            jobCard.DateCompleted = DateTime.UtcNow;

            await _jobCardRepository.UpdateAsync(jobCard);

            await _jobCardRepository.SaveChangesAsync();

            //---------------------------------------------------
            // Audit
            //---------------------------------------------------

            if (!string.Equals(
                oldStatus,
                "Completed",
                StringComparison.OrdinalIgnoreCase))
            {
                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    performedByUserId,
                    JobCardAuditAction.JobCardCompleted,
                    $"Status changed from '{oldStatus}' to 'Completed'",
                    oldStatus,
                    "Completed");
            }
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
            int performedByUserId)
        {
            var jobCard =
                await _jobCardRepository.GetByIdAsync(id);

            if (jobCard == null)
                return;

            //---------------------------------------------------
            // Audit deletion before removing the record
            //---------------------------------------------------

            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.JobCardDeleted,
                $"Job Card {jobCard.JobNumber} deleted");

            //---------------------------------------------------
            // Delete
            //---------------------------------------------------

            await _jobCardRepository.DeleteAsync(jobCard);

            await _jobCardRepository.SaveChangesAsync();
        }


        //---------------------------------------------------
        // Add Labour
        //---------------------------------------------------

        public async Task AddLabourEntryAsync(
    int jobCardId,
    AddLabourEntryDto dto,
    int performedByUserId)
        {
            var jobCard =
                await _jobCardRepository.GetByIdAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            //---------------------------------------------------
            // Job Card must have an assigned technician
            //---------------------------------------------------

            if (jobCard.AssignedTechnicianId == null)
                throw new Exception("No technician assigned.");

            //---------------------------------------------------
            // Create Labour Entry
            //---------------------------------------------------

            var labour = new JobCardLabour
            {
                JobCardId = jobCardId,

                // The authenticated user who created the entry.
                CreatedByUserId = performedByUserId,

                HoursWorked = dto.HoursWorked,

                WorkPerformed = dto.WorkPerformed,

                DateWorked = DateTime.UtcNow
            };

            //---------------------------------------------------
            // Save
            //---------------------------------------------------

            await _jobCardRepository.AddLabourEntryAsync(labour);

            await _jobCardRepository.SaveChangesAsync();

            //---------------------------------------------------
            // Audit
            //---------------------------------------------------

            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.LabourAdded,
                $"Added {dto.HoursWorked} hours of labour.");
        }
        //---------------------------------------------------
        // Get Labour
        //---------------------------------------------------

        public async Task<List<JobCardLabour>> GetLabourEntriesAsync(
            int jobCardId)
        {
            return await _jobCardRepository
                .GetLabourEntriesAsync(jobCardId);
        }
        

        //---------------------------------------------
        //Update labour 
        //---------------------------------------------
        public async Task UpdateLabourEntryAsync(
     int jobCardId,
     int labourId,
     UpdateLabourEntryDto dto,
     int performedByUserId,
     string role)


        {
            var labour =
                await _jobCardRepository.GetLabourEntryByIdAsync(labourId);

            if (labour == null)
                throw new Exception("Labour entry not found.");

            if (labour.JobCardId != jobCardId)
                throw new Exception("Labour entry does not belong to this Job Card.");

            var jobCard = labour.JobCard;

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            //---------------------------------------------------
            // Technician permissions
            //---------------------------------------------------

            if (string.Equals(
                role,
                "Technician",
                StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(
                    jobCard.Status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception(
                        "Technicians cannot modify labour on a completed Job Card.");
                }

                if (labour.CreatedByUserId != performedByUserId)
                {
                    throw new Exception(
                        "You can only modify your own labour entries.");
                }
            }

            //---------------------------------------------------
            // Store original values
            //---------------------------------------------------

            var oldHoursWorked = labour.HoursWorked;
            var oldWorkPerformed = labour.WorkPerformed;
            var oldDateWorked = labour.DateWorked;

            //---------------------------------------------------
            // Update
            //---------------------------------------------------

            labour.HoursWorked = dto.HoursWorked;
            labour.WorkPerformed = dto.WorkPerformed;
            labour.DateWorked = dto.DateWorked;

            await _jobCardRepository.UpdateLabourEntryAsync(labour);

            await _jobCardRepository.SaveChangesAsync();

            //---------------------------------------------------
            // Audit
            //---------------------------------------------------

            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.LabourUpdated,
                $"Labour entry updated. Hours changed from " +
                $"{oldHoursWorked} to {dto.HoursWorked}.",
                oldHoursWorked.ToString(),
                dto.HoursWorked.ToString());
        }

        //------------------------------------
        //Delete labour 
        //------------------------------------



        public async Task DeleteLabourEntryAsync(
     int jobCardId,
     int labourId,
     int performedByUserId,
     string role)
        {
            var labour =
                await _jobCardRepository.GetLabourEntryByIdAsync(labourId);

            if (labour == null)
                throw new Exception("Labour entry not found.");

            if (labour.JobCardId != jobCardId)
                throw new Exception("Labour entry does not belong to this Job Card.");

            var jobCard = labour.JobCard;

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            //---------------------------------------------------
            // Technician permissions
            //---------------------------------------------------

            if (string.Equals(
                role,
                "Technician",
                StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(
                    jobCard.Status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception(
                        "Technicians cannot delete labour from a completed Job Card.");
                }

                if (labour.CreatedByUserId != performedByUserId)
                {
                    throw new Exception(
                        "You can only delete your own labour entries.");
                }
            }

            //---------------------------------------------------
            // Audit
            //---------------------------------------------------

            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.LabourDeleted,
                $"Deleted labour entry: " +
                $"{labour.HoursWorked} hours - {labour.WorkPerformed}");

            //---------------------------------------------------
            // Delete
            //---------------------------------------------------

            await _jobCardRepository.DeleteLabourEntryAsync(labour);

            await _jobCardRepository.SaveChangesAsync();
        }
        //---------------------------------------------------
        // Add Part
        //---------------------------------------------------

        public async Task AddPartAsync(
            int jobCardId,
            AddPartDto dto,
            int performedByUserId,
            string role)   // 👈 NEW: add role parameter
        {
            var jobCard =
                await _jobCardRepository.GetByIdAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            //---------------------------------------------------
            // Technician permissions
            //---------------------------------------------------
            if (string.Equals(role, "Technician", StringComparison.OrdinalIgnoreCase))
            {
                // Technicians cannot add parts to completed Job Cards
                if (string.Equals(jobCard.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Technicians cannot add parts to a completed Job Card.");
                }

                // Technicians can only add parts to their assigned Job Cards
                if (jobCard.AssignedTechnicianId != performedByUserId)
                {
                    throw new Exception("You can only add parts to Job Cards assigned to you.");
                }
            }

            //---------------------------------------------------
            // Validate quantity
            //---------------------------------------------------
            if (dto.Quantity <= 0)
            {
                throw new Exception("Quantity must be greater than zero.");
            }

            //---------------------------------------------------
            // Validate part name
            //---------------------------------------------------
            if (string.IsNullOrWhiteSpace(dto.PartName))
            {
                throw new Exception("Part name is required.");
            }

            //---------------------------------------------------
            // Create Part
            //---------------------------------------------------
            var part = new JobCardPart
            {
                JobCardId = jobCardId,
                PartName = dto.PartName.Trim(),
                Quantity = dto.Quantity
            };

            await _jobCardRepository.AddPartAsync(part);
            await _jobCardRepository.SaveChangesAsync();

            //---------------------------------------------------
            // Audit
            //---------------------------------------------------
            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.PartAdded,
                $"Added part '{part.PartName}' x{part.Quantity}");
        }

        //---------------------------------------------------
        // Get Parts
        //---------------------------------------------------

        public async Task<List<JobCardPartDto>> GetPartsAsync(
            int jobCardId)
        {
            var parts =
                await _jobCardRepository.GetPartsAsync(jobCardId);

            return parts
                .Select(p => new JobCardPartDto
                {
                    PartId = p.PartId,
                    PartName = p.PartName,
                    Quantity = p.Quantity
                })
                .ToList();
        }



        //---------------------------------------------------
        // Update Part
        //---------------------------------------------------
        public async Task UpdatePartAsync(
            int jobCardId,
            int partId,
            UpdatePartDto dto,
            int performedByUserId,
            string role)
        {
            //---------------------------------------------------
            // Find Part
            //---------------------------------------------------
            var part = await _jobCardRepository.GetPartByIdAsync(partId);
            if (part == null)
                throw new Exception("Part not found.");

            //---------------------------------------------------
            // Make sure Part belongs to supplied Job Card
            //---------------------------------------------------
            if (part.JobCardId != jobCardId)
                throw new Exception("Part does not belong to this Job Card.");

            //---------------------------------------------------
            // Find Job Card
            //---------------------------------------------------
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                throw new Exception("Job Card not found.");

            //---------------------------------------------------
            // Technician permissions
            //---------------------------------------------------
            if (string.Equals(role, "Technician", StringComparison.OrdinalIgnoreCase))
            {
                // Technicians cannot modify completed Job Cards
                if (string.Equals(jobCard.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                    throw new Exception("Technicians cannot modify parts on a completed Job Card.");

                // Technicians can only modify their assigned Job Cards
                if (jobCard.AssignedTechnicianId != performedByUserId)
                    throw new Exception("You can only modify parts on Job Cards assigned to you.");
            }

            //---------------------------------------------------
            // Validate Part Name
            //---------------------------------------------------
            if (string.IsNullOrWhiteSpace(dto.PartName))
                throw new Exception("Part name is required.");

            //---------------------------------------------------
            // Validate Quantity
            //---------------------------------------------------
            if (dto.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

            //---------------------------------------------------
            // Store original values
            //---------------------------------------------------
            var oldPartName = part.PartName;
            var oldQuantity = part.Quantity;

            //---------------------------------------------------
            // Update
            //---------------------------------------------------
            part.PartName = dto.PartName.Trim();
            part.Quantity = dto.Quantity;

            await _jobCardRepository.UpdatePartAsync(part);
            await _jobCardRepository.SaveChangesAsync();

            //---------------------------------------------------
            // Audit
            //---------------------------------------------------
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



        //---------------------------------------------------
        // Delete Part
        //---------------------------------------------------
        public async Task DeletePartAsync(
            int partId,
            int performedByUserId,
            string role)
        {
            var part = await _jobCardRepository.GetPartByIdAsync(partId);
            if (part == null)
                throw new Exception("Part not found.");

            var jobCard = await _jobCardRepository.GetByIdAsync(part.JobCardId);
            if (jobCard == null)
                throw new Exception("Job Card not found.");

            //---------------------------------------------------
            // Technician permissions
            //---------------------------------------------------
            if (string.Equals(role, "Technician", StringComparison.OrdinalIgnoreCase))
            {
                // Technicians cannot delete parts from completed Job Cards
                if (string.Equals(jobCard.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Technicians cannot delete parts from a completed Job Card.");
                }

                // Technicians can only delete parts on their assigned Job Cards
                if (jobCard.AssignedTechnicianId != performedByUserId)
                {
                    throw new Exception("You can only delete parts on Job Cards assigned to you.");
                }
            }

            //---------------------------------------------------
            // Audit
            //---------------------------------------------------
            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.PartDeleted,
                $"Deleted part '{part.PartName}' x{part.Quantity}");

            //---------------------------------------------------
            // Delete
            //---------------------------------------------------
            await _jobCardRepository.DeletePartAsync(part);
            await _jobCardRepository.SaveChangesAsync();
        }


        //---------------------------------------------------
        // Get By Ticket Id
        //---------------------------------------------------

        public async Task<JobCard?> GetByTicketIdAsync(int ticketId)
        {
            return await _jobCardRepository.GetByTicketIdAsync(ticketId);
        }
    }
}
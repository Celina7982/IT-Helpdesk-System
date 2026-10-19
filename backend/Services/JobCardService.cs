using IThelpdesk.Data;
using IThelpdesk.DTOs.Common;
using IThelpdesk.DTOs.JobCard;
using IThelpdesk.Enums;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;
using Microsoft.EntityFrameworkCore;
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
        private readonly ISlaTicketRepository _slaTicketRepository;
        private readonly ApplicationDbContext _context;


        public JobCardService(
            IJobCardRepository jobCardRepository,
            ITicketRepository ticketRepository,
            IJobCardAuditService auditService,
            IUserRepository userRepository,
            INotificationService notificationService,
            ISlaTicketRepository slaTicketRepository,
            ApplicationDbContext context)

        {
            _jobCardRepository = jobCardRepository;
            _ticketRepository = ticketRepository;
            _auditService = auditService;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _slaTicketRepository = slaTicketRepository;
            _context = context;
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
            //--------------------------------------------------
            // Get Ticket
            //--------------------------------------------------

            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found.");


            //--------------------------------------------------
            // Ticket must be Resolved
            //--------------------------------------------------

            if (!string.Equals(
                    ticket.Status,
                    "Resolved",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "A Job Card can only be created after the ticket has been resolved.");
            }


            //--------------------------------------------------
            // Check if Job Card already exists
            //--------------------------------------------------

            var existing =
                await _jobCardRepository.GetByTicketIdAsync(ticketId);

            if (existing != null)
            {
                return existing;
            }


            //--------------------------------------------------
            // Prevent Job Card if SLA Report already exists
            //--------------------------------------------------

            var existingSla =
                await _slaTicketRepository.GetByTicketIdAsync(ticketId);

            if (existingSla != null)
            {
                throw new Exception(
                    $"Ticket #{ticketId} already has SLA Report {existingSla.SlaNumber}. " +
                    "A Job Card cannot also be created for this ticket.");
            }


            //--------------------------------------------------
            // Generate Job Card Number
            //--------------------------------------------------

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


            //--------------------------------------------------
            // Create Job Card
            //--------------------------------------------------


            var jobCard = new JobCard
            {
                //--------------------------------------------------
                // Digital Job Card
                //--------------------------------------------------

                JobCardType = JobCardType.Digital,

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


            //--------------------------------------------------
            // Save Job Card
            //--------------------------------------------------

            await _jobCardRepository.AddAsync(jobCard);
            await _jobCardRepository.SaveChangesAsync();


            //--------------------------------------------------
            // Audit Log
            //--------------------------------------------------

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
            // Notify Assigned Technician
            //--------------------------------------------------

            if (jobCard.AssignedTechnicianId.HasValue)
            {
                await _notificationService.CreateAsync(
                    jobCard.AssignedTechnicianId.Value,
                    "New Job Card Assigned",
                    $"Job Card {jobCard.JobNumber} has been created and assigned to you.");
            }


            //--------------------------------------------------
            // Return Job Card
            //--------------------------------------------------

            return jobCard;
        }


        //---------------------------------------------------
        // Create Paper Job Card
        //---------------------------------------------------

        public async Task<JobCard> CreatePaperJobCardAsync(
            CreatePaperJobCardDto dto,
            int performedByUserId)
        {
            //--------------------------------------------------
            // Validate Paper Job Card Information
            //--------------------------------------------------

            if (dto == null)
                throw new ArgumentException("Job card information is required.");

            if (string.IsNullOrWhiteSpace(dto.PaperJobCardNumber))
                throw new ArgumentException("Paper job card number is required.");

            if (!dto.PaperJobCardDate.HasValue)
                throw new ArgumentException("Paper job card date is required.");

            if (string.IsNullOrWhiteSpace(dto.CustomerName))
                throw new ArgumentException("Customer name is required.");

            if (string.IsNullOrWhiteSpace(dto.FaultReported))
                throw new ArgumentException("Fault reported is required.");

            //--------------------------------------------------
            // Validate Optional Ticket
            //--------------------------------------------------

            if (dto.TicketId.HasValue)
            {
                var ticket = await _ticketRepository
                    .GetByIdAsync(dto.TicketId.Value);

                if (ticket == null)
                    throw new ArgumentException("Ticket not found.");

                var existingJobCard = await _jobCardRepository
                    .GetByTicketIdAsync(dto.TicketId.Value);

                if (existingJobCard != null)
                    throw new ArgumentException(
                        "This ticket already has a job card.");

                var existingSla = await _slaTicketRepository
                    .GetByTicketIdAsync(dto.TicketId.Value);

                if (existingSla != null)
                    throw new ArgumentException(
                        "This ticket already has an SLA report.");
            }

            //--------------------------------------------------
            // Validate Initial Lead Technician
            //--------------------------------------------------

            // Every Paper Job Card must have a Lead Technician
            if (!dto.AssignedTechnicianId.HasValue)
            {
                throw new ArgumentException(
                    "A Lead Technician is required for every Paper Job Card.");
            }

            // Check that the selected user exists
            var leadTechnician = await _userRepository
                .GetUserEntityByIdAsync(
                    dto.AssignedTechnicianId.Value);

            if (leadTechnician == null)
            {
                throw new ArgumentException(
                    "The selected Lead Technician was not found.");
            }

            // Only Admins and Technicians can be assigned
            bool isAdmin = string.Equals(
                leadTechnician.Role,
                "Admin",
                StringComparison.OrdinalIgnoreCase);

            bool isTechnician = string.Equals(
                leadTechnician.Role,
                "Technician",
                StringComparison.OrdinalIgnoreCase);

            if (!isAdmin && !isTechnician)
            {
                throw new ArgumentException(
                    "The Lead Technician must have the Admin or Technician role.");
            }

            //--------------------------------------------------
            // Generate Internal Job Number
            //--------------------------------------------------

            var latest = await _jobCardRepository
                .GetLatestJobCardAsync();

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
                $"JC-{DateTime.UtcNow.Year}-{nextNumber:D6}";

            //--------------------------------------------------
            // Create Paper Job Card
            //--------------------------------------------------

            var jobCard = new JobCard
            {
                JobCardType = JobCardType.Paper,

                JobNumber = jobNumber,

                PaperJobCardNumber =
                    dto.PaperJobCardNumber.Trim(),

                PaperJobCardDate =
                    dto.PaperJobCardDate.Value,

                TicketId = dto.TicketId,

                AssignedTechnicianId =
                    dto.AssignedTechnicianId,

                CapturedByUserId = performedByUserId,

                DateCaptured = DateTime.UtcNow,

                DateCreated = DateTime.UtcNow,

                Status = "Open",

                CustomerName = dto.CustomerName.Trim(),

                FaultReported = dto.FaultReported.Trim(),

                FaultFound = dto.FaultFound ?? "",

                WorkPerformed = dto.WorkPerformed ?? "",

                CompletionNotes = dto.CompletionNotes ?? "",

                CustomerSignature = "",

                SignedDate = null
            };




            //--------------------------------------------------
            // Save Paper Job Card and Assigned Technicians
            //--------------------------------------------------

            // Start with the selected technicians.
            // Include the Lead Technician automatically.
            var technicianIds = (dto.AssignedTechnicianIds ?? new List<int>())
                .Append(dto.AssignedTechnicianId.Value)
                .Distinct()
                .ToList();

            //--------------------------------------------------
            // Validate all selected technicians before saving
            //--------------------------------------------------

            foreach (var technicianId in technicianIds)
            {
                var technician = await _userRepository
                    .GetUserEntityByIdAsync(technicianId);

                if (technician == null ||
                    !(string.Equals(
                        technician.Role,
                        "Admin",
                        StringComparison.OrdinalIgnoreCase)
                      ||
                      string.Equals(
                        technician.Role,
                        "Technician",
                        StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ArgumentException(
                        $"User {technicianId} is not a valid Admin or Technician.");
                }
            }

            //--------------------------------------------------
            // Create Paper Job Card
            //--------------------------------------------------

            await _jobCardRepository.AddAsync(jobCard);

            //--------------------------------------------------
            // Create all technician assignments
            //--------------------------------------------------

            foreach (var technicianId in technicianIds)
            {
                var assignment = new JobCardTechnician
                {
                    JobCard = jobCard,

                    TechnicianId = technicianId,

                    IsLeadTechnician =
                        technicianId == dto.AssignedTechnicianId.Value,

                    AssignedByUserId = performedByUserId,

                    DateAssigned = DateTime.UtcNow
                };

                await _jobCardRepository
                    .AddTechnicianAssignmentAsync(assignment);
            }

            //--------------------------------------------------
            // Save Job Card and assignments together
            //--------------------------------------------------

            await _jobCardRepository.SaveChangesAsync();




            


            //--------------------------------------------------
            // Record Existing Job Card Audit History
            //--------------------------------------------------

            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.JobCardCreated,
                $"Paper Job Card {jobCard.PaperJobCardNumber} " +
                $"registered as {jobCard.JobNumber}."
            );

            //--------------------------------------------------
            // Notify Assigned Technician
            //--------------------------------------------------

            if (jobCard.AssignedTechnicianId.HasValue)
            {
                await _notificationService.CreateAsync(
                    jobCard.AssignedTechnicianId.Value,
                    "Paper Job Card Assigned",
                    $"Paper Job Card {jobCard.JobNumber} " +
                    "has been assigned to you."
                );
            }

            return jobCard;
        }


        //---------------------------------------------------
        // Paper Job Card - Assign Technician
        //---------------------------------------------------



        public async Task AssignPaperJobCardTechnicianAsync(
            int jobCardId,
            int technicianId,
            bool isLeadTechnician,
            int performedByUserId)
        {
            var executionStrategy =
                _context.Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                // Start each attempt with fresh tracked entities.
                _context.ChangeTracker.Clear();

                await using var transaction =
                    await _context.Database.BeginTransactionAsync();

                try
                {
                    // Verify the user performing the assignment.
                    var assigningUser = await _userRepository
                        .GetUserEntityByIdAsync(performedByUserId);

                    if (assigningUser == null)
                    {
                        throw new UnauthorizedAccessException(
                            "The user performing this assignment was not found.");
                    }

                    // Find the Job Card.
                    var jobCard = await _context.JobCards
                        .SingleOrDefaultAsync(j =>
                            j.JobCardId == jobCardId);

                    if (jobCard == null)
                        throw new ArgumentException("Job Card not found.");

                    // Never modify Digital Job Cards.
                    if (jobCard.JobCardType != JobCardType.Paper)
                    {
                        throw new InvalidOperationException(
                            "Technician assignments through this method " +
                            "are only supported for Paper Job Cards.");
                    }

                    bool isAdmin = string.Equals(
                        assigningUser.Role,
                        "Admin",
                        StringComparison.OrdinalIgnoreCase);

                    var assignments = await _jobCardRepository
                        .GetTrackedTechnicianAssignmentsAsync(jobCardId);

                    bool isLead = assignments.Any(a =>
                        a.TechnicianId == performedByUserId &&
                        a.IsLeadTechnician);

                    // Only Admin or Lead can add technicians.
                    if (!isAdmin && !isLead)
                    {
                        throw new UnauthorizedAccessException(
                            "Only Admins or the Lead Technician can assign engineers.");
                    }

                    // Only Admin can appoint a Lead.
                    if (isLeadTechnician && !isAdmin)
                    {
                        throw new UnauthorizedAccessException(
                            "Only Admins can appoint a Lead Technician.");
                    }

                    bool isCompleted = string.Equals(
                        jobCard.Status,
                        "Completed",
                        StringComparison.OrdinalIgnoreCase);

                    // Only Admin can assign after completion.
                    if (isCompleted && !isAdmin)
                    {
                        throw new UnauthorizedAccessException(
                            "Only Admin can assign technicians to a completed Paper Job Card.");
                    }

                    // Validate the selected technician.
                    var technician = await _userRepository
                        .GetUserEntityByIdAsync(technicianId);

                    if (technician == null)
                    {
                        throw new ArgumentException(
                            "Selected technician was not found.");
                    }

                    bool validRole =
                        string.Equals(
                            technician.Role,
                            "Admin",
                            StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(
                            technician.Role,
                            "Technician",
                            StringComparison.OrdinalIgnoreCase);

                    if (!validRole)
                    {
                        throw new InvalidOperationException(
                            "Only Admins and Technicians can be assigned to a Paper Job Card.");
                    }

                    // Prevent duplicate assignments.
                    if (assignments.Any(a =>
                        a.TechnicianId == technicianId))
                    {
                        throw new InvalidOperationException(
                            "This technician is already assigned to the Paper Job Card.");
                    }

                    // Every Paper Job Card already has one Lead.
                    // Use ChangePaperLeadTechnicianAsync to replace that Lead.
                    if (isLeadTechnician)
                    {
                        throw new InvalidOperationException(
                            "To change the Lead Technician, first assign the " +
                            "technician as Secondary, then use Change Lead Technician.");
                    }

                    // Create a Secondary Technician assignment.
                    var assignment = new JobCardTechnician
                    {
                        JobCardId = jobCardId,
                        TechnicianId = technicianId,
                        IsLeadTechnician = false,
                        AssignedByUserId = performedByUserId,
                        DateAssigned = DateTime.UtcNow
                    };

                    await _jobCardRepository
                        .AddTechnicianAssignmentAsync(assignment);

                    await _jobCardRepository.SaveChangesAsync();

                    // Record assignment in audit history.
                    await _auditService.LogAsync(
                        jobCardId,
                        performedByUserId,
                        JobCardAuditAction.PaperTechnicianAssigned,
                        $"Technician {technician.FirstName} {technician.LastName} " +
                        $"(User ID: {technicianId}) was assigned to " +
                        $"Paper Job Card {jobCard.JobNumber}.");

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }



        public async Task ChangePaperLeadTechnicianAsync(
            int jobCardId,
            int newLeadTechnicianId,
            int performedByUserId)
        {
            var executionStrategy =
                _context.Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                // Reload database entities on every attempt.
                // This prevents retries from reusing changed entities.
                _context.ChangeTracker.Clear();

                await using var transaction =
                    await _context.Database.BeginTransactionAsync();

                try
                {
                    // Only Admin can change the Lead Technician.
                    var currentUser = await _userRepository
                        .GetUserEntityByIdAsync(performedByUserId);

                    if (currentUser == null ||
                        !string.Equals(
                            currentUser.Role,
                            "Admin",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        throw new UnauthorizedAccessException(
                            "Only Admin can change the Lead Technician.");
                    }

                    // Retrieve the Paper Job Card as a tracked entity.
                    var jobCard = await _context.JobCards
                        .SingleOrDefaultAsync(j =>
                            j.JobCardId == jobCardId);

                    if (jobCard == null)
                        throw new ArgumentException("Job Card not found.");

                    if (jobCard.JobCardType != JobCardType.Paper)
                    {
                        throw new InvalidOperationException(
                            "Lead Technician changes are only supported for Paper Job Cards.");
                    }

                    // Validate the selected new Lead.
                    var newLeadUser = await _userRepository
                        .GetUserEntityByIdAsync(newLeadTechnicianId);

                    if (newLeadUser == null ||
                        !(string.Equals(
                            newLeadUser.Role,
                            "Admin",
                            StringComparison.OrdinalIgnoreCase) ||
                          string.Equals(
                            newLeadUser.Role,
                            "Technician",
                            StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new ArgumentException(
                            "The new Lead must be an Admin or Technician.");
                    }

                    // Retrieve tracked technician assignments.
                    var assignments = await _jobCardRepository
                        .GetTrackedTechnicianAssignmentsAsync(jobCardId);

                    var leadAssignments = assignments
                        .Where(a => a.IsLeadTechnician)
                        .ToList();

                    // Every Paper Job Card must have exactly one Lead.
                    if (leadAssignments.Count != 1)
                    {
                        throw new InvalidOperationException(
                            "This Paper Job Card must have exactly one Lead Technician.");
                    }

                    var currentLead = leadAssignments[0];

                    // Ensure both Lead references agree.
                    if (jobCard.AssignedTechnicianId !=
                        currentLead.TechnicianId)
                    {
                        throw new InvalidOperationException(
                            "The Paper Job Card Lead Technician does not match " +
                            "the assigned Lead Technician.");
                    }

                    // A retry may occur after a successful commit.
                    // In that case, no further changes are necessary.
                    if (currentLead.TechnicianId == newLeadTechnicianId)
                    {
                        await transaction.CommitAsync();
                        return;
                    }

                    // The new Lead must already be assigned.
                    var newLeadAssignment = assignments
                        .SingleOrDefault(a =>
                            a.TechnicianId == newLeadTechnicianId);

                    if (newLeadAssignment == null)
                    {
                        throw new InvalidOperationException(
                            "Assign the new Lead Technician to this Job Card first.");
                    }

                    var oldLeadUser = await _userRepository
                        .GetUserEntityByIdAsync(currentLead.TechnicianId);

                    string oldLeadName = oldLeadUser != null
                        ? $"{oldLeadUser.FirstName} {oldLeadUser.LastName}"
                        : $"User {currentLead.TechnicianId}";

                    // Demote the old Lead.
                    currentLead.IsLeadTechnician = false;

                    // Promote the new Lead.
                    newLeadAssignment.IsLeadTechnician = true;

                    // Keep the main Job Card Lead synchronized.
                    jobCard.AssignedTechnicianId =
                        newLeadTechnicianId;

                    await _context.SaveChangesAsync();

                    // Save audit history within the same transaction.
                    await _auditService.LogAsync(
                        jobCardId,
                        performedByUserId,
                        JobCardAuditAction.PaperLeadTechnicianChanged,
                        $"Lead Technician changed from {oldLeadName} " +
                        $"to {newLeadUser.FirstName} {newLeadUser.LastName}.",
                        currentLead.TechnicianId.ToString(),
                        newLeadTechnicianId.ToString());

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }



        public async Task RemovePaperJobCardTechnicianAsync(
            int jobCardId,
            int technicianId,
            int performedByUserId)
        {
            var executionStrategy =
                _context.Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                // Start every attempt with fresh database entities.
                _context.ChangeTracker.Clear();

                await using var transaction =
                    await _context.Database.BeginTransactionAsync();

                try
                {
                    // Only Admin can remove technicians.
                    var performedByUser = await _userRepository
                        .GetUserEntityByIdAsync(performedByUserId);

                    bool isAdmin = performedByUser != null &&
                        string.Equals(
                            performedByUser.Role,
                            "Admin",
                            StringComparison.OrdinalIgnoreCase);

                    if (!isAdmin)
                    {
                        throw new UnauthorizedAccessException(
                            "Only Admin can remove assigned technicians.");
                    }

                    // Retrieve the Job Card.
                    var jobCard = await _context.JobCards
                        .SingleOrDefaultAsync(j =>
                            j.JobCardId == jobCardId);

                    if (jobCard == null)
                    {
                        throw new ArgumentException(
                            "Job Card not found.");
                    }

                    // Digital Job Cards must remain unchanged.
                    if (jobCard.JobCardType != JobCardType.Paper)
                    {
                        throw new InvalidOperationException(
                            "Technician removal is only supported for Paper Job Cards.");
                    }

                    // Retrieve the assigned technicians.
                    var assignments = await _jobCardRepository
                        .GetTrackedTechnicianAssignmentsAsync(jobCardId);

                    var assignment = assignments
                        .SingleOrDefault(a =>
                            a.TechnicianId == technicianId);

                    if (assignment == null)
                    {
                        throw new ArgumentException(
                            "Technician is not assigned to this Job Card.");
                    }

                    // Never remove the current Lead Technician.
                    if (assignment.IsLeadTechnician ||
                        jobCard.AssignedTechnicianId == technicianId)
                    {
                        throw new InvalidOperationException(
                            "The current Lead Technician cannot be removed. " +
                            "Change the Lead Technician first, then remove " +
                            "the previous Lead if required.");
                    }

                    // Get the technician's name for audit history.
                    var removedTechnician = await _userRepository
                        .GetUserEntityByIdAsync(technicianId);

                    string technicianName = removedTechnician != null
                        ? $"{removedTechnician.FirstName} {removedTechnician.LastName}"
                        : $"User {technicianId}";

                    // Remove only the assignment.
                    // Existing Work Completed and Parts remain.
                    await _jobCardRepository
                        .RemoveTechnicianAssignmentAsync(assignment);

                    await _jobCardRepository.SaveChangesAsync();

                    // Record the removal in audit history.
                    await _auditService.LogAsync(
                        jobCardId,
                        performedByUserId,
                        JobCardAuditAction.PaperTechnicianRemoved,
                        $"Technician {technicianName} " +
                        $"(User ID: {technicianId}) was removed from " +
                        $"Paper Job Card {jobCard.JobNumber}.");

                    // Commit assignment removal and audit together.
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
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


    //--------------------------------------------------
    // Paper Job Card - Overall Time Tracking
    //--------------------------------------------------

    public async Task UpdatePaperJobTimeAsync(
        int jobCardId,
        UpdatePaperJobTimeDto dto,
        int performedByUserId)
        {
            //--------------------------------------------------
            // 1. Find Job Card
            //--------------------------------------------------

            var jobCard =
                await _jobCardRepository.GetByIdAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            //--------------------------------------------------
            // 2. Ensure this is a Paper Job Card
            //--------------------------------------------------

            if (jobCard.JobCardType != JobCardType.Paper)
                throw new Exception(
                    "Time tracking through this endpoint is only available for Paper Job Cards.");



            //--------------------------------------------------
            // 3. Check user permissions
            //--------------------------------------------------

            var user = await _userRepository
                .GetUserEntityByIdAsync(performedByUserId);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "User not found.");
            }

            bool isAdmin = string.Equals(
                user.Role,
                "Admin",
                StringComparison.OrdinalIgnoreCase);

            // The Lead Technician is stored on the Job Card.
            bool isLeadTechnician =
                jobCard.AssignedTechnicianId == performedByUserId;

            // Only Admin or Lead can update overall time.
            if (!isAdmin && !isLeadTechnician)
            {
                throw new UnauthorizedAccessException(
                    "Only Admin or the Lead Technician can update " +
                    "Paper Job Card overall time.");
            }

            //--------------------------------------------------
            // 4. Check Job Card status
            //--------------------------------------------------

            bool isCompleted = string.Equals(
                jobCard.Status,
                "Completed",
                StringComparison.OrdinalIgnoreCase);

            // Once completed, only Admin may update overall time.
            if (isCompleted && !isAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only Admin can update overall time on a " +
                    "completed Paper Job Card.");
            }


            //--------------------------------------------------
            // 5. Validate Time In and Time Out
            //--------------------------------------------------



            if (dto.TimeOut.HasValue && !dto.TimeIn.HasValue)
            {
                throw new ArgumentException(
                    "Time In is required when Time Out is provided.");
            }

            if (dto.TimeIn.HasValue &&
                dto.TimeOut.HasValue &&
                dto.TimeOut.Value < dto.TimeIn.Value)
            {
                throw new Exception(
                    "Time Out cannot be earlier than Time In.");
            }



            //--------------------------------------------------
            // Remember original values for audit history
            //--------------------------------------------------

            var oldTimeIn = jobCard.TimeIn;
            var oldTimeOut = jobCard.TimeOut;
            var oldIsTravel = jobCard.IsTravel;
            var oldIsOnSite = jobCard.IsOnSite;

            //--------------------------------------------------
            // 6. Update overall Job Card time
            //--------------------------------------------------

            jobCard.TimeIn = dto.TimeIn;
            jobCard.TimeOut = dto.TimeOut;

            jobCard.IsTravel = dto.IsTravel;
            jobCard.IsOnSite = dto.IsOnSite;

            //--------------------------------------------------
            // 7. Save changes
            //--------------------------------------------------

            await _jobCardRepository.UpdateAsync(jobCard);

            await _jobCardRepository.SaveChangesAsync();


            //--------------------------------------------------
            // 8. Record Paper Job Card time changes == AUDIT ACTION
            //--------------------------------------------------

            bool timeChanged =
                oldTimeIn != jobCard.TimeIn ||
                oldTimeOut != jobCard.TimeOut ||
                oldIsTravel != jobCard.IsTravel ||
                oldIsOnSite != jobCard.IsOnSite;

            if (timeChanged)
            {
                string oldValues =
                    $"Time In: {oldTimeIn:O}, " +
                    $"Time Out: {oldTimeOut:O}, " +
                    $"Travel: {oldIsTravel}, " +
                    $"On Site: {oldIsOnSite}";

                string newValues =
                    $"Time In: {jobCard.TimeIn:O}, " +
                    $"Time Out: {jobCard.TimeOut:O}, " +
                    $"Travel: {jobCard.IsTravel}, " +
                    $"On Site: {jobCard.IsOnSite}";

                await _auditService.LogAsync(
                    jobCard.JobCardId,
                    performedByUserId,
                    JobCardAuditAction.PaperJobTimeUpdated,
                    "Paper Job Card overall time information updated",
                    oldValues,
                    newValues);
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

            //--------------------------------------------------
            // Paper Job Card Completion Permissions
            //--------------------------------------------------

            if (jobCard.JobCardType == JobCardType.Paper)
            {
                var currentUser = await _userRepository
                    .GetUserEntityByIdAsync(performedByUserId);

                if (currentUser == null)
                    throw new UnauthorizedAccessException(
                        "User not found.");

                bool isAdmin = string.Equals(
                    currentUser.Role,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase);

                bool isAssignedTechnician =
                    jobCard.AssignedTechnicianId == performedByUserId;


                if (!isAssignedTechnician)
                {
                    isAssignedTechnician = await _jobCardRepository
                        .IsTechnicianAssignedAsync(
                            id,
                            performedByUserId);
                }


                if (!isAdmin && !isAssignedTechnician)
                {
                    throw new UnauthorizedAccessException(
                        "Only Admin or an assigned technician can complete this Paper Job Card.");
                }

                if (string.Equals(
                    jobCard.Status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "This Paper Job Card is already completed.");
                }
            }

            //--------------------------------------------------
            // Complete Job Card
            //--------------------------------------------------

            var oldStatus = jobCard.Status;

            jobCard.Status = "Completed";
            jobCard.DateCompleted = DateTime.UtcNow;

            await _jobCardRepository.UpdateAsync(jobCard);
            await _jobCardRepository.SaveChangesAsync();

            //--------------------------------------------------
            // Audit Completion
            //--------------------------------------------------

            await _auditService.LogAsync(
                jobCard.JobCardId,
                performedByUserId,
                JobCardAuditAction.JobCardCompleted,
                $"Job Card {jobCard.JobNumber} completed.",
                oldStatus,
                "Completed");
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

        //---------------------------------------------------
        // Paper Job Card - Add Work Completed
        //---------------------------------------------------

        public async Task AddPaperWorkEntryAsync(
            int jobCardId,
            AddPaperWorkEntryDto dto,
            int performedByUserId)
        {
            // 1. Find the Job Card
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            // 2. Ensure this is a Paper Job Card
            if (jobCard.JobCardType != JobCardType.Paper)
                throw new Exception(
                    "Work Completed entries can only be added using this method for Paper Job Cards.");

           
            //--------------------------------------------------
            // Check permission to capture work
            //--------------------------------------------------

            var capturingUser =
    await _userRepository.GetUserEntityByIdAsync(performedByUserId);



            if (capturingUser == null)
                throw new UnauthorizedAccessException(
                    "User not found.");

            bool isAdmin = capturingUser.Role == "Admin";


            //--------------------------------------------------
            // Company rule:
            // Only Admin can add work after completion.
            //--------------------------------------------------

            if (string.Equals(
                    jobCard.Status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase)
                && !isAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only Admin can add Work Completed to a completed Paper Job Card.");
            }


            bool isCapturingTechnicianAssigned =
                await _jobCardRepository.IsTechnicianAssignedAsync(
                    jobCardId,
                    performedByUserId);

            if (!isAdmin && !isCapturingTechnicianAssigned)
                throw new UnauthorizedAccessException(
                    "Only an Admin or assigned Technician can capture work for this Paper Job Card.");


            // 4. Ensure the responsible technician is assigned
            var isAssigned =
                await _jobCardRepository.IsTechnicianAssignedAsync(
                    jobCardId,
                    dto.TechnicianId);

            if (!isAssigned)
                throw new Exception(
                    "The responsible technician must be assigned to this Paper Job Card.");



            //--------------------------------------------------
            // Company permission rules:
            // Admin and Lead Technician can capture for
            // any assigned technician.
            // Secondary Technician can capture for self only.
            //--------------------------------------------------

            bool isLeadTechnician =
                jobCard.AssignedTechnicianId == performedByUserId;

            if (!isAdmin &&
                !isLeadTechnician &&
                dto.TechnicianId != performedByUserId)
            {
                throw new UnauthorizedAccessException(
                    "Secondary technicians can only capture Work Completed for themselves.");
            }

            // 5. Create the work entry
            var workEntry = new JobCardLabour
            {
                JobCardId = jobCardId,

                // Person responsible for the work
                TechnicianId = dto.TechnicianId,

                // Person who entered the information
                CapturedByUserId = performedByUserId,

                WorkPerformed = dto.WorkPerformed.Trim(),

                // Paper Job Cards do not require labour hours
                HoursWorked = 0,

                DateWorked = DateTime.UtcNow,

                IsPaperWorkEntry = true
            };

            // 6. Save to SQL Server
            await _jobCardRepository.AddLabourEntryAsync(workEntry);
            await _jobCardRepository.SaveChangesAsync();

            // 7. Record audit history
            await _auditService.LogAsync(
                jobCardId,
                performedByUserId,
                JobCardAuditAction.LabourAdded,
                $"Added Paper Job Card work entry for technician {dto.TechnicianId}.");
        }


        public async Task<List<JobCardLabourEntryDto>> GetLabourEntriesAsync(int jobCardId)
        {
            var labourEntries =
                await _jobCardRepository.GetLabourEntriesAsync(jobCardId);

            var result = new List<JobCardLabourEntryDto>();

            foreach (var entry in labourEntries)
            {
                //--------------------------------------------------
                // Get responsible technician
                //--------------------------------------------------

                var technician =
                    await _userRepository.GetUserEntityByIdAsync(
                        entry.TechnicianId);

                string technicianName = technician != null
                    ? $"{technician.FirstName} {technician.LastName}"
                    : $"Technician {entry.TechnicianId}";

                //--------------------------------------------------
                // Get user who captured the work
                //--------------------------------------------------

                string? capturedByUserName = null;

                if (entry.CapturedByUserId.HasValue)
                {
                    var capturedByUser =
                        await _userRepository.GetUserEntityByIdAsync(
                            entry.CapturedByUserId.Value);

                    capturedByUserName = capturedByUser != null
                        ? $"{capturedByUser.FirstName} {capturedByUser.LastName}"
                        : $"User {entry.CapturedByUserId.Value}";
                }

                //--------------------------------------------------
                // Build response
                //--------------------------------------------------

                result.Add(new JobCardLabourEntryDto
                {
                    LabourId = entry.LabourId,
                    JobCardId = entry.JobCardId,

                    TechnicianId = entry.TechnicianId,
                    TechnicianName = technicianName,

                    HoursWorked = entry.HoursWorked,
                    WorkPerformed = entry.WorkPerformed,
                    DateWorked = entry.DateWorked,

                    // Paper Job Card fields
                    CapturedByUserId = entry.CapturedByUserId,
                    CapturedByUserName = capturedByUserName,
                    IsPaperWorkEntry = entry.IsPaperWorkEntry
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
        }


        //---------------------------------------------------
        // Paper Job Card - Add Parts Used
        //---------------------------------------------------

        public async Task AddPaperPartAsync(
            int jobCardId,
            AddPaperPartDto dto,
            int performedByUserId)
        {
            //--------------------------------------------------
            // 1. Find the Job Card
            //--------------------------------------------------

            var jobCard =
                await _jobCardRepository.GetByIdAsync(jobCardId);

            if (jobCard == null)
                throw new Exception("Job Card not found.");

            //--------------------------------------------------
            // 2. Ensure this is a Paper Job Card
            //--------------------------------------------------

            if (jobCard.JobCardType != JobCardType.Paper)
                throw new Exception(
                    "Parts can only be added using this method for Paper Job Cards.");



            //--------------------------------------------------
            // 4. Validate the part details
            //--------------------------------------------------

            if (string.IsNullOrWhiteSpace(dto.PartName))
                throw new Exception("Part name is required.");

            if (dto.PartName.Trim().Length > 150)
                throw new Exception(
                    "Part name cannot exceed 150 characters.");

            if (dto.Quantity < 1 || dto.Quantity > 9999)
                throw new Exception(
                    "Quantity must be between 1 and 9999.");

            //--------------------------------------------------
            // 5. Check the user capturing the part
            //--------------------------------------------------

            var capturingUser =
                await _userRepository.GetUserEntityByIdAsync(
                    performedByUserId);

            if (capturingUser == null)
                throw new UnauthorizedAccessException(
                    "User not found.");

            bool isAdmin =
                string.Equals(
                    capturingUser.Role,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase);


            bool isCompleted =
                string.Equals(
                    jobCard.Status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase);

            // Only Admin can add Parts after completion.
            if (isCompleted && !isAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only Admin can add Parts to a completed Paper Job Card.");
            }


            bool isAssigned =
                await _jobCardRepository.IsTechnicianAssignedAsync(
                    jobCardId,
                    performedByUserId);

            if (!isAdmin && !isAssigned)
                throw new UnauthorizedAccessException(
                    "Only an Admin or assigned Technician can capture parts for this Paper Job Card.");

            //--------------------------------------------------
            // 6. Check the responsible technician
            //--------------------------------------------------

            bool responsibleTechnicianAssigned =
                await _jobCardRepository.IsTechnicianAssignedAsync(
                    jobCardId,
                    dto.ResponsibleTechnicianId);

            if (!responsibleTechnicianAssigned)
                throw new Exception(
                    "The responsible technician must be assigned to this Paper Job Card.");


            //--------------------------------------------------
            // 7. Enforce Lead / Secondary permissions
            //--------------------------------------------------

            if (!isAdmin &&
                dto.ResponsibleTechnicianId != performedByUserId)
            {
                var assignments =
                    await _jobCardRepository.GetAssignedTechniciansAsync(
                        jobCardId);

                bool isLeadTechnician = assignments.Any(
                    assignment =>
                        assignment.TechnicianId == performedByUserId &&
                        assignment.IsLeadTechnician);

                if (!isLeadTechnician)
                {
                    throw new UnauthorizedAccessException(
                        "Secondary Technicians can only record Parts for themselves.");
                }
            }


            //--------------------------------------------------
            // 7. Create the Paper Part entry
            //--------------------------------------------------

            var part = new JobCardPart
            {
                JobCardId = jobCardId,

                PartName = dto.PartName.Trim(),

                Quantity = dto.Quantity,

                ResponsibleTechnicianId =
                    dto.ResponsibleTechnicianId,

                CapturedByUserId = performedByUserId,

                CreatedByUserId =
                    performedByUserId.ToString(),

                DateAdded = DateTime.UtcNow,

                IsPaperPartEntry = true
            };

            //--------------------------------------------------
            // 8. Save the part to SQL Server
            //--------------------------------------------------

            await _jobCardRepository.AddPartAsync(part);
            await _jobCardRepository.SaveChangesAsync();

            //--------------------------------------------------
            // 9. Record audit history
            //--------------------------------------------------

            await _auditService.LogAsync(
                jobCardId,
                performedByUserId,
                JobCardAuditAction.PartAdded,
                $"Added Paper Job Card part '{part.PartName}' x{part.Quantity} for technician {dto.ResponsibleTechnicianId}.");
        }



        public async Task<List<JobCardPartDto>> GetPartsAsync(int jobCardId)
        {
            var parts = await _jobCardRepository.GetPartsAsync(jobCardId);

            var result = new List<JobCardPartDto>();

            foreach (var p in parts)
            {
                //--------------------------------------------------
                // 1. Existing Digital Job Card - Added By
                //--------------------------------------------------

                int.TryParse(p.CreatedByUserId, out int createdById);

                var user = createdById > 0
                    ? await _userRepository.GetUserEntityByIdAsync(createdById)
                    : null;

                string addedBy = user != null
                    ? $"{user.FirstName} {user.LastName}"
                    : $"User {p.CreatedByUserId}";

                //--------------------------------------------------
                // 2. Paper Job Card - Responsible Technician
                //--------------------------------------------------

                string? responsibleTechnicianName = null;

                if (p.ResponsibleTechnicianId.HasValue)
                {
                    var technician =
                        await _userRepository.GetUserEntityByIdAsync(
                            p.ResponsibleTechnicianId.Value);

                    responsibleTechnicianName = technician != null
                        ? $"{technician.FirstName} {technician.LastName}"
                        : $"Technician {p.ResponsibleTechnicianId.Value}";
                }

                //--------------------------------------------------
                // 3. Paper Job Card - Captured By
                //--------------------------------------------------

                string? capturedByUserName = null;

                if (p.CapturedByUserId.HasValue)
                {
                    var capturedByUser =
                        await _userRepository.GetUserEntityByIdAsync(
                            p.CapturedByUserId.Value);

                    capturedByUserName = capturedByUser != null
                        ? $"{capturedByUser.FirstName} {capturedByUser.LastName}"
                        : $"User {p.CapturedByUserId.Value}";
                }

                //--------------------------------------------------
                // 4. Build response
                //--------------------------------------------------

                result.Add(new JobCardPartDto
                {
                    // Existing Digital Job Card fields
                    PartId = p.PartId,
                    JobCardId = p.JobCardId,
                    PartName = p.PartName,
                    Quantity = p.Quantity,
                    AddedByName = addedBy,
                    AddedByUserId = createdById,
                    DateAdded = p.DateAdded,

                    // Paper Job Card fields
                    IsPaperPartEntry = p.IsPaperPartEntry,

                    ResponsibleTechnicianId =
                        p.ResponsibleTechnicianId,

                    ResponsibleTechnicianName =
                        responsibleTechnicianName,

                    CapturedByUserId = p.CapturedByUserId,

                    CapturedByUserName = capturedByUserName
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
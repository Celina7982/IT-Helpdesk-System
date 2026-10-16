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
        private readonly ISlaTicketRepository _slaTicketRepository;
        public JobCardService(
    IJobCardRepository jobCardRepository,
    ITicketRepository ticketRepository,
    IJobCardAuditService auditService,
    IUserRepository userRepository,
    INotificationService notificationService,
    ISlaTicketRepository slaTicketRepository)
        {
            _jobCardRepository = jobCardRepository;
            _ticketRepository = ticketRepository;
            _auditService = auditService;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _slaTicketRepository = slaTicketRepository;
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
            // Save Paper Job Card and Initial Lead Technician
            //--------------------------------------------------

            // Add the Paper Job Card
            await _jobCardRepository.AddAsync(jobCard);

            // Create its initial Lead Technician assignment
            var leadAssignment = new JobCardTechnician
            {
                JobCard = jobCard,

                TechnicianId = dto.AssignedTechnicianId.Value,

                IsLeadTechnician = true,

                AssignedByUserId = performedByUserId,

                DateAssigned = DateTime.UtcNow
            };

            // Add the Lead Technician assignment
            await _jobCardRepository.AddTechnicianAssignmentAsync(
                leadAssignment
            );

            // Save both records together
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
            // Verify the user performing the assignment
            // Admins and Lead Technicians can assign engineers

            var assigningUser = await _userRepository
                .GetUserEntityByIdAsync(performedByUserId);

            if (assigningUser == null)
            {
                throw new UnauthorizedAccessException(
                    "The user performing this assignment was not found.");
            }

            

            // Check that the Job Card exists
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);

            if (jobCard == null)
                throw new ArgumentException("Job Card not found.");

            //--------------------------------------------------
            // Admin or Lead Technician can assign engineers
            //--------------------------------------------------

            bool isAdmin = string.Equals(
                assigningUser.Role,
                "Admin",
                StringComparison.OrdinalIgnoreCase);

            bool isLead = false;

            if (!isAdmin)
            {
                var assignments = await _jobCardRepository
                    .GetAssignedTechniciansAsync(jobCardId);

                isLead = assignments.Any(t =>
                    t.TechnicianId == performedByUserId &&
                    t.IsLeadTechnician);
            }

            if (!isAdmin && !isLead)
            {
                throw new UnauthorizedAccessException(
                    "Only Admins or the Lead Technician can assign engineers.");
            }

            // Only Admins can appoint a Lead Technician
            if (isLeadTechnician && !isAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Only Admins can appoint a Lead Technician.");
            }

            // This method is only for Paper Job Cards
            if (jobCard.JobCardType != JobCardType.Paper)
                throw new InvalidOperationException(
                    "Technician assignments through this method are only supported for Paper Job Cards.");

            // Do not change assignments after completion
            if (string.Equals(
                jobCard.Status,
                "Completed",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Cannot assign technicians to a completed Paper Job Card.");
            }

            // Verify that the user exists
            var technician = await _userRepository
                .GetUserEntityByIdAsync(technicianId);

            if (technician == null)
                throw new ArgumentException("Selected technician was not found.");

            //--------------------------------------------------
            // Only Admins and Technicians can be assigned
            //--------------------------------------------------

            if (!string.Equals(
                    technician.Role,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase)
                &&
                !string.Equals(
                    technician.Role,
                    "Technician",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Only Admins and Technicians can be assigned to a Paper Job Card.");
            }

            // Prevent duplicate assignments
            var alreadyAssigned = await _jobCardRepository
                .IsTechnicianAssignedAsync(jobCardId, technicianId);

            if (alreadyAssigned)
                throw new InvalidOperationException(
                    "This technician is already assigned to the Paper Job Card.");

            // Only one lead technician is allowed
            if (isLeadTechnician)
            {
                var assignedTechnicians = await _jobCardRepository
                    .GetAssignedTechniciansAsync(jobCardId);

                if (assignedTechnicians.Any(t => t.IsLeadTechnician))
                {
                    throw new InvalidOperationException(
                        "This Paper Job Card already has a lead technician.");
                }
            }

            // Create the assignment
            var assignment = new JobCardTechnician
            {
                JobCardId = jobCardId,
                TechnicianId = technicianId,
                IsLeadTechnician = isLeadTechnician,
                AssignedByUserId = performedByUserId,
                DateAssigned = DateTime.UtcNow
            };

            await _jobCardRepository
                .AddTechnicianAssignmentAsync(assignment);

            await _jobCardRepository.SaveChangesAsync();
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
            // 3. Check Job Card status
            //--------------------------------------------------

            if (string.Equals(
                jobCard.Status,
                "Completed",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Cannot update time tracking on a completed Paper Job Card.");
            }

            //--------------------------------------------------
            // 4. Check user permissions
            //--------------------------------------------------

            var user =
                await _userRepository.GetUserEntityByIdAsync(
                    performedByUserId);

            if (user == null)
                throw new UnauthorizedAccessException(
                    "User not found.");

            bool isAdmin = user.Role == "Admin";

            bool isAssignedTechnician =
                await _jobCardRepository.IsTechnicianAssignedAsync(
                    jobCardId,
                    performedByUserId);

            if (!isAdmin && !isAssignedTechnician)
                throw new UnauthorizedAccessException(
                    "Only an Admin or assigned Technician can update Paper Job Card time.");




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

            // 3. Prevent changes to completed Job Cards
            if (jobCard.Status == "Completed")
                throw new Exception(
                    "Cannot add work to a completed Paper Job Card.");
            //--------------------------------------------------
            // Check permission to capture work
            //--------------------------------------------------

            var capturingUser =
    await _userRepository.GetUserEntityByIdAsync(performedByUserId);



            if (capturingUser == null)
                throw new UnauthorizedAccessException(
                    "User not found.");

            bool isAdmin = capturingUser.Role == "Admin";

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
            // 3. Prevent changes to completed Paper Job Cards
            //--------------------------------------------------

            if (string.Equals(
                jobCard.Status,
                "Completed",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Cannot add parts to a completed Paper Job Card.");
            }

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
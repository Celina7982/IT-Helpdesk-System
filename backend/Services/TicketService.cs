using IThelpdesk.DTOs.Ticket;
using IThelpdesk.Enums;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;
using IThelpdesk.DTOs;
using IThelpdesk.Entities;
using IThelpdesk.Repositories;

namespace IThelpdesk.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IJobCardRepository _jobCardRepository;
        private readonly IJobCardAuditService _auditService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;

        public TicketService(
    ITicketRepository ticketRepository,
    IJobCardRepository jobCardRepository,
    IJobCardAuditService auditService,
    INotificationService notificationService,
    IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _jobCardRepository = jobCardRepository;
            _auditService = auditService;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        //-------------------------------------------------------
        // Ticket Lists
        //-------------------------------------------------------

        public async Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _ticketRepository.GetAllAsync(pageNumber, pageSize);
        }

        public async Task<IEnumerable<Ticket>> GetAvailableTicketsAsync()
        {
            return await _ticketRepository.GetAvailableTicketsAsync();
        }

        public async Task<IEnumerable<TicketResponseDto>> GetMyTicketsAsync(
     int technicianId, int pageNumber = 1, int pageSize = 10)
        {
            return await _ticketRepository.GetMyTicketsAsync(technicianId, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Ticket>> GetEscalatedTicketsAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _ticketRepository.GetEscalatedTicketsAsync(pageNumber, pageSize);
        }

        public async Task<IEnumerable<TicketResponseDto>> GetArchivedTicketsAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _ticketRepository.GetArchivedTicketsAsync(pageNumber, pageSize);
        }


        public async Task<IEnumerable<Ticket>> GetMyTicketsByUserAsync(int userId)
        {
            return await _ticketRepository.GetMyTicketsByUserAsync(userId);
        }

        //-------------------------------------------------------
        // Single Ticket
        //-------------------------------------------------------

        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            return await _ticketRepository.GetByIdAsync(id);
        }

        public async Task<TicketDetailsDto?> GetTicketDetailsAsync(int id)
        {
            return await _ticketRepository.GetTicketDetailsAsync(id);
        }

        //-------------------------------------------------------
        // CRUD
        //-------------------------------------------------------

        public async Task CreateTicketAsync(Ticket ticket)
        {
            await _ticketRepository.AddAsync(ticket);
            await _ticketRepository.SaveChangesAsync();
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            await _ticketRepository.UpdateAsync(ticket);
            await _ticketRepository.SaveChangesAsync();
        }

        public async Task DeleteTicketAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket == null)
                return;

            await _ticketRepository.DeleteAsync(ticket);
            await _ticketRepository.SaveChangesAsync();
        }

        //-------------------------------------------------------
        // Assign Ticket
        //-------------------------------------------------------

        public async Task AssignTicketAsync(
     int ticketId,
     int assignedToUserId,
     int assignedByUserId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null) // ticket null check
                throw new Exception("Ticket not found.");

            // -------------------------------------------------------
            // Validate the user receiving the ticket
            // Only active Admins and Technicians may be assigned.
            // -------------------------------------------------------

            var assignedUser =
                await _userRepository.GetUserEntityByIdAsync(assignedToUserId);

            if (assignedUser == null)
            {
                throw new Exception("Selected user was not found.");
            }

            if (!assignedUser.IsActive)
            {
                throw new Exception("Tickets cannot be assigned to an inactive user.");
            }

            if (assignedUser.Role != "Admin" &&
                assignedUser.Role != "Technician")
            {
                throw new Exception(
                    "Tickets can only be assigned to an Admin or Technician."
                );
            }


            //-------------------------------------------------------
            // Validate ticket status
            //-------------------------------------------------------

            if (ticket.IsArchived ||
                ticket.Status == "Resolved")
            {
                throw new InvalidOperationException(
                    "Resolved or archived tickets cannot be reassigned.");
            }

            //-------------------------------------------------------
            // Store previous primary assignee
            //-------------------------------------------------------

            var oldTechnicianId = ticket.AssignedToUserId;

            //-------------------------------------------------------
            // Synchronize primary assignment
            //-------------------------------------------------------

            // Check whether this user already has an assignment
            // in the new TicketAssignments table.

            var alreadyAssigned =
                await _ticketRepository.HasTicketAssignmentAsync(
                    ticketId,
                    assignedToUserId);

            // Only create a new record if one does not exist.

            if (!alreadyAssigned)
            {
                // The existing AssignTicketAsync signature does not
                // receive the logged-in Admin ID.
                // This value is temporary until we pass the actual
                // Admin ID from the controller.

                var assignment = new TicketAssignment
                {
                    TicketId = ticketId,
                    UserId = assignedToUserId,
                    AssignedByUserId = assignedByUserId,
                    AssignedDate = DateTime.UtcNow
                };

                await _ticketRepository.AddTicketAssignmentAsync(
                    assignment);
            }


            // -------------------------------------------------------
            // Update Primary Technician And Ticket Status
            // -------------------------------------------------------

            ticket.AssignedToUserId = assignedToUserId;
            ticket.Status = "In Progress";
            ticket.IsEscalated = false;

            // Preserve EscalationReason for history.

            // -------------------------------------------------------
            // Optimistic Concurrency Protection
            // -------------------------------------------------------

            await _ticketRepository.MarkTicketForConcurrencyCheckAsync(ticket);

            // -------------------------------------------------------
            // Save Primary Assignment And TicketAssignment
            // -------------------------------------------------------

       
            await _ticketRepository.SaveChangesAsync();

            //-------------------------------------------------------
            // Notify Customer
            //-------------------------------------------------------

            var technician =
      await _ticketRepository.GetUserByIdAsync(
          assignedToUserId);

            var technicianName = technician != null
                ? $"{technician.FirstName} {technician.LastName}"
                : "a technician";

            await _notificationService.CreateAsync(
                ticket.UserId,
                "Ticket Assigned",
                $"Your ticket '{ticket.Subject}' has been assigned to {technicianName}.");

            //-------------------------------------------------------
            // Notify Assigned Technician
            //-------------------------------------------------------

            await _notificationService.CreateAsync(
                assignedToUserId,
                "Ticket Assigned",
                $"Ticket #{ticket.TicketId} has been assigned to you.",
                ticket.TicketId);

            //-------------------------------------------------------
            // Update Job Card technician if one exists
            //-------------------------------------------------------

            var jobCard = await _jobCardRepository.GetByTicketIdAsync(ticketId);

            if (jobCard != null)
            {
                jobCard.AssignedTechnicianId = assignedToUserId;

                await _jobCardRepository.UpdateAsync(jobCard);
                await _jobCardRepository.SaveChangesAsync();

                //-------------------------------------------------------
                // Audit technician reassignment
                //-------------------------------------------------------

                if (oldTechnicianId.HasValue &&
                    oldTechnicianId.Value != assignedToUserId)
                {
                    var oldTech =
                        await _ticketRepository.GetUserByIdAsync(
                            oldTechnicianId.Value);

                    var newTech =
                        await _ticketRepository.GetUserByIdAsync(
                            assignedToUserId);

                    string oldName = oldTech != null
                        ? $"{oldTech.FirstName} {oldTech.LastName}"
                        : "Unassigned";

                    string newName = newTech != null
                        ? $"{newTech.FirstName} {newTech.LastName}"
                        : "Unknown";

                    await _auditService.LogAsync(
                        jobCard.JobCardId,
                        assignedToUserId,
                        JobCardAuditAction.TechnicianReassigned,
                        $"Technician reassigned from '{oldName}' to '{newName}'",
                        oldName,
                        newName);
                }
            }
        }


        // ----------------------------------------------------
        // Multiple Ticket Assignments
        // ----------------------------------------------------

        public async Task<List<TicketAssigneeDto>> GetTicketAssigneesAsync(
            int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException("Ticket not found.");
            }

            var assignments =
                await _ticketRepository.GetTicketAssignmentsAsync(ticketId);

            return assignments
                .Select(a => new TicketAssigneeDto
                {
                    UserId = a.UserId,

                    FullName = a.User?.FullName ?? "Unknown User",

                    Role = a.User?.Role ?? "",

                    AssignedDate = a.AssignedDate,

                    AssignedByUserId = a.AssignedByUserId,

                    AssignedByName =
                        a.AssignedByUser?.FullName ?? "Unknown User"
                })
                .ToList();
        }


        // ----------------------------------------------------
        // Add User To Ticket
        // Admin calls this method from the controller
        // ----------------------------------------------------

        public async Task AddTicketAssigneeAsync(
            int ticketId,
            int userId,
            int assignedByUserId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException("Ticket not found.");
            }


            // Assignment changes are not allowed after resolution
            if (ticket.Status == "Resolved")
            {
                throw new InvalidOperationException(
                    "Resolved tickets cannot be reassigned.");
            }


            // Assignment changes are not allowed on archived tickets
            if (ticket.IsArchived)
            {
                throw new InvalidOperationException(
                    "Archived tickets cannot be reassigned.");
            }


            var user = await _ticketRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "The selected user does not exist.");
            }


            if (!user.IsActive)
            {
                throw new InvalidOperationException(
                    "Inactive users cannot be assigned to tickets.");
            }


            if (user.Role != "Admin" &&
                user.Role != "Technician")
            {
                throw new InvalidOperationException(
                    "Only Admins and Technicians can be assigned to tickets.");
            }


            var alreadyAssigned =
                await _ticketRepository.HasTicketAssignmentAsync(
                    ticketId,
                    userId);

            if (alreadyAssigned)
            {
                throw new InvalidOperationException(
                    "This user is already assigned to the ticket.");
            }


            var assignment = new TicketAssignment
            {
                TicketId = ticketId,
                UserId = userId,
                AssignedByUserId = assignedByUserId,
                AssignedDate = DateTime.UtcNow
            };




            await _ticketRepository.AddTicketAssignmentAsync(assignment);

            // ----------------------------------------------------
            // Set the primary assignee only if none exists
            // ----------------------------------------------------

            if (ticket.AssignedToUserId == null)
            {
                ticket.AssignedToUserId = userId;
            }

            // ----------------------------------------------------
            // Update ticket lifecycle
            // ----------------------------------------------------

            // Once someone is assigned, the ticket becomes
            // In Progress.

            ticket.Status = "In Progress";

            // Admin assignment clears the escalated flag.
            ticket.IsEscalated = false;

            // Preserve EscalationReason for history.

            // ----------------------------------------------------
            // Save assignment and ticket changes
            // with optimistic concurrency protection
            // ----------------------------------------------------

            // Force EF Core to perform a concurrency-checked
            // UPDATE on the parent Ticket record.
            //
            // This is important even when the ticket is already
            // "In Progress" and the primary Technician is unchanged.

            await _ticketRepository.MarkTicketForConcurrencyCheckAsync(ticket);

            // Save the Ticket and TicketAssignment changes together.
            // EF Core will check the original RowVersion during UPDATE.

            await _ticketRepository.SaveChangesAsync();

        }



        public async Task RemoveTicketAssigneeAsync(
            int ticketId,
            int userId,
            int removedByUserId)
        {
            // ----------------------------------------------------
            // Get Ticket
            // ----------------------------------------------------

            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException("Ticket not found.");
            }

            // ----------------------------------------------------
            // Validate Ticket Status
            // ----------------------------------------------------

            if (ticket.Status == "Resolved")
            {
                throw new InvalidOperationException(
                    "Assignments cannot be changed on a resolved ticket.");
            }

            if (ticket.IsArchived)
            {
                throw new InvalidOperationException(
                    "Assignments cannot be changed on an archived ticket.");
            }

            // ----------------------------------------------------
            // Find Assignment To Remove
            // ----------------------------------------------------

            var assignment =
                await _ticketRepository.GetTicketAssignmentAsync(
                    ticketId,
                    userId);

            if (assignment == null)
            {
                throw new KeyNotFoundException(
                    "This user is not assigned to the ticket.");
            }

            // ----------------------------------------------------
            // Load Current Assignments
            // ----------------------------------------------------

            var currentAssignments =
                await _ticketRepository.GetTicketAssignmentsAsync(ticketId);

            // Exclude the user being removed.
            // Remaining users are ordered by assignment date.

            var remainingAssignments = currentAssignments
                .Where(a => a.UserId != userId)
                .OrderBy(a => a.AssignedDate)
                .ThenBy(a => a.TicketAssignmentId)
                .ToList();

            // ----------------------------------------------------
            // Determine Whether Primary Is Being Removed
            // ----------------------------------------------------

            bool removingPrimary =
                ticket.AssignedToUserId == userId;

            // ----------------------------------------------------
            // Remove Assignment
            // ----------------------------------------------------

            await _ticketRepository.RemoveTicketAssignmentAsync(
                assignment);

            // ----------------------------------------------------
            // Update Primary Assignee If Necessary
            // ----------------------------------------------------

            if (removingPrimary)
            {
                if (remainingAssignments.Any())
                {
                    // Promote the next remaining assigned user.

                    ticket.AssignedToUserId =
                        remainingAssignments.First().UserId;
                }
                else
                {
                    // No assigned users remain.

                    ticket.AssignedToUserId = null;
                }
            }

            // ----------------------------------------------------
            // Handle Ticket With No Remaining Assignees
            // ----------------------------------------------------

            if (!remainingAssignments.Any())
            {
                ticket.AssignedToUserId = null;

                // Return to Open unless the ticket is escalated.

                if (!ticket.IsEscalated &&
                    ticket.Status != "Escalated")
                {
                    ticket.Status = "Open";
                }
            }

            // ----------------------------------------------------
            // Save Ticket And Assignment Changes
            // With Optimistic Concurrency Protection
            // ----------------------------------------------------


            await _ticketRepository.MarkTicketForConcurrencyCheckAsync(ticket);

            

            await _ticketRepository.SaveChangesAsync();

            // ----------------------------------------------------
            //: Job Card Ownership
            // ----------------------------------------------------

            // Do not update JobCard.AssignedTechnicianId here.
            // Ticket collaboration and Job Card ownership
            // are managed separately.
        }



        // -------------------------------------------------------
        // Claim Ticket
        // -------------------------------------------------------

        public async Task ClaimTicketAsync(
            int ticketId,
            int technicianId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }

            // -------------------------------------------------------
            // Validate ticket availability
            // -------------------------------------------------------

            if (ticket.IsArchived ||
                ticket.Status == "Resolved")
            {
                throw new Exception(
                    "Archived or resolved tickets cannot be claimed.");
            }

            if (ticket.IsEscalated ||
                ticket.Status == "Escalated")
            {
                throw new Exception(
                    "Escalated tickets cannot be claimed. An administrator must assign the ticket.");
            }

            if (ticket.AssignedToUserId != null)
            {
                throw new Exception("Ticket already assigned.");
            }

            // Check the new assignment table as well.
            // This prevents claiming a ticket that already
            // has one or more assigned users.

            var existingAssignments =
                await _ticketRepository.GetTicketAssignmentsAsync(ticketId);

            if (existingAssignments.Any())
            {
                throw new Exception("Ticket already assigned.");
            }

            // -------------------------------------------------------
            // Validate the technician
            // -------------------------------------------------------

            var technician =
                await _ticketRepository.GetUserByIdAsync(technicianId);

            if (technician == null ||
                !technician.IsActive ||
                (technician.Role != "Technician" &&
                 technician.Role != "Admin"))
            {
                throw new Exception(
                    "Only active Technicians or Admins can claim tickets.");
            }

            // -------------------------------------------------------
            // Create the new assignment record
            // -------------------------------------------------------

            var assignment = new TicketAssignment
            {
                TicketId = ticketId,
                UserId = technicianId,

                // Claiming is self-assignment.
                AssignedByUserId = technicianId,

                AssignedDate = DateTime.UtcNow
            };

            await _ticketRepository.AddTicketAssignmentAsync(assignment);

            // -------------------------------------------------------
            // Update Primary Technician And Ticket Status
            // -------------------------------------------------------

            ticket.AssignedToUserId = technicianId;
            ticket.Status = "In Progress";

            // -------------------------------------------------------
            // Optimistic Concurrency Protection
            // -------------------------------------------------------


            await _ticketRepository.MarkTicketForConcurrencyCheckAsync(ticket);

            // -------------------------------------------------------
            // Save Ticket And Assignment Together
            // -------------------------------------------------------

          
            await _ticketRepository.SaveChangesAsync();

            // -------------------------------------------------------
            // Notify Customer
            // -------------------------------------------------------

            var technicianName =
                $"{technician.FirstName} {technician.LastName}";

            await _notificationService.CreateAsync(
                ticket.UserId,
                "Ticket Assigned",
                $"Your ticket '{ticket.Subject}' has been assigned to {technicianName}.");
        }


        //-------------------------------------------------------
        // Escalate Ticket
        //-------------------------------------------------------

        public async Task EscalateTicketAsync(
    int ticketId,
    string escalationReason)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found.");

            //-------------------------------------------------------
            // Get Technician Before Removing Assignment
            //-------------------------------------------------------

            User? technician = null;

            if (ticket.AssignedToUserId.HasValue)
            {
                technician =
                    await _userRepository.GetUserEntityByIdAsync(
                        ticket.AssignedToUserId.Value);
            }

            var technicianName =
                technician != null
                    ? $"{technician.FirstName} {technician.LastName}"
                    : "a technician";


            //-------------------------------------------------------
            // Remove All Current Ticket Assignments
            //-------------------------------------------------------

            // Load everyone currently assigned to the ticket.

            var currentAssignments =
                await _ticketRepository.GetTicketAssignmentsAsync(ticketId);

            // Remove each assignment from the new table.
            // This prevents escalated tickets from remaining
            // assigned to technicians or other users.

            foreach (var assignment in currentAssignments)
            {
                // Get a tracked assignment entity before deleting.
                var trackedAssignment =
                    await _ticketRepository.GetTicketAssignmentAsync(
                        ticketId,
                        assignment.UserId);

                if (trackedAssignment != null)
                {
                    await _ticketRepository.RemoveTicketAssignmentAsync(
                        trackedAssignment);
                }
            }

            //-------------------------------------------------------
            // Escalate Ticket
            //-------------------------------------------------------

            ticket.Status = "Escalated";
            ticket.IsEscalated = true;
            ticket.EscalationReason = escalationReason;

            // Clear the primary assignee.
            ticket.AssignedToUserId = null;


            //-------------------------------------------------------
            // Save Escalation With Concurrency Protection
            //-------------------------------------------------------

            

            await _ticketRepository.MarkTicketForConcurrencyCheckAsync(ticket);


            await _ticketRepository.SaveChangesAsync();

            //-------------------------------------------------------
            // Notify Customer
            //-------------------------------------------------------

            await _notificationService.CreateAsync(
                ticket.UserId,
                "Ticket Escalated",
                $"Your ticket '{ticket.Subject}' has been escalated for further attention.",
                ticket.TicketId);

            //-------------------------------------------------------
            // Notify All Admins
            //-------------------------------------------------------

            var admins =
                await _userRepository.GetAdminsAsync();

            foreach (var admin in admins)
            {
                await _notificationService.CreateAsync(
                    admin.UserId,
                    "Ticket Escalated",
                    $"Ticket #{ticket.TicketId} has been escalated by {technicianName}.",
                    ticket.TicketId);
            }
        }

        //-------------------------------------------------------
        // Resolve Ticket
        //-------------------------------------------------------

        public async Task ResolveTicketAsync(
            int ticketId,
            int userId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found.");

            ticket.Status = "Resolved";

            ticket.IsEscalated = false;
            ticket.EscalationReason = null;

            await _ticketRepository.UpdateAsync(ticket);
            await _ticketRepository.SaveChangesAsync();

            //-------------------------------------------------------
            // Notify Customer
            //-------------------------------------------------------

            await _notificationService.CreateAsync(
                ticket.UserId,
                "Ticket Resolved",
                $"Your ticket '{ticket.Subject}' has been resolved.");
        }

        //-------------------------------------------------------
        // Archive Ticket
        //-------------------------------------------------------

        public async Task ArchiveTicketAsync(int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found.");

            ticket.IsArchived = true;
            ticket.ArchivedDate = DateTime.UtcNow;

            await _ticketRepository.ArchiveAsync(ticket);
            await _ticketRepository.SaveChangesAsync();
        }

     

       
    }
}
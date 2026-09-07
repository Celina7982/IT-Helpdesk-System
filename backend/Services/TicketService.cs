using IThelpdesk.DTOs.Ticket;
using IThelpdesk.Enums;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;

namespace IThelpdesk.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IJobCardRepository _jobCardRepository;
        private readonly IJobCardAuditService _auditService;
        private readonly INotificationService _notificationService;

        public TicketService(
            ITicketRepository ticketRepository,
            IJobCardRepository jobCardRepository,
            IJobCardAuditService auditService,
            INotificationService notificationService)
        {
            _ticketRepository = ticketRepository;
            _jobCardRepository = jobCardRepository;
            _auditService = auditService;
            _notificationService = notificationService;
        }

        //-------------------------------------------------------
        // Ticket Lists
        //-------------------------------------------------------

        public async Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync()
        {
            return await _ticketRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Ticket>> GetAvailableTicketsAsync()
        {
            return await _ticketRepository.GetAvailableTicketsAsync();
        }

        public async Task<IEnumerable<TicketResponseDto>> GetMyTicketsAsync(
            int technicianId)
        {
            return await _ticketRepository.GetMyTicketsAsync(technicianId);
        }

        public async Task<IEnumerable<Ticket>> GetEscalatedTicketsAsync()
        {
            return await _ticketRepository.GetEscalatedTicketsAsync();
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
            int assignedToUserId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found.");

            //-------------------------------------------------------
            // Store previous technician
            //-------------------------------------------------------

            var oldTechnicianId = ticket.AssignedToUserId;

            //-------------------------------------------------------
            // Update ticket
            //-------------------------------------------------------

            ticket.AssignedToUserId = assignedToUserId;
            ticket.Status = "In Progress";
            ticket.IsEscalated = false;
            ticket.EscalationReason = null;

            await _ticketRepository.UpdateAsync(ticket);
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

        //-------------------------------------------------------
        // Claim Ticket
        //-------------------------------------------------------

        public async Task ClaimTicketAsync(
            int ticketId,
            int technicianId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found.");

            if (ticket.AssignedToUserId != null)
                throw new Exception("Ticket already assigned.");

            ticket.AssignedToUserId = technicianId;
            ticket.Status = "In Progress";

            await _ticketRepository.UpdateAsync(ticket);
            await _ticketRepository.SaveChangesAsync();

            //-------------------------------------------------------
            // Notify Customer
            //-------------------------------------------------------

            var technician =
                await _ticketRepository.GetUserByIdAsync(
                    technicianId);

            var technicianName = technician != null
                ? $"{technician.FirstName} {technician.LastName}"
                : "a technician";

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

            ticket.Status = "Escalated";
            ticket.IsEscalated = true;
            ticket.EscalationReason = escalationReason;
            ticket.AssignedToUserId = null;

            await _ticketRepository.UpdateAsync(ticket);
            await _ticketRepository.SaveChangesAsync();

            //-------------------------------------------------------
            // Notify Customer
            //-------------------------------------------------------

            await _notificationService.CreateAsync(
                ticket.UserId,
                "Ticket Escalated",
                $"Your ticket '{ticket.Subject}' has been escalated for further attention.");
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

        //-------------------------------------------------------
        // Archived Tickets
        //-------------------------------------------------------

        public async Task<IEnumerable<TicketResponseDto>> GetArchivedTicketsAsync()
        {
            return await _ticketRepository.GetArchivedTicketsAsync();
        }
    }
}
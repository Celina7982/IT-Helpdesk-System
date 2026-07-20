using IThelpdesk.DTOs.Ticket;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;

namespace IThelpdesk.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
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

        public async Task<IEnumerable<Ticket>> GetMyTicketsAsync(int technicianId)
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

        public async Task AssignTicketAsync(int ticketId, int assignedToUserId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found.");

            ticket.AssignedToUserId = assignedToUserId;
            ticket.Status = "In Progress";
            ticket.IsEscalated = false;
            ticket.EscalationReason = null;

            await _ticketRepository.UpdateAsync(ticket);
            await _ticketRepository.SaveChangesAsync();
        }

        //-------------------------------------------------------
        // Claim Ticket
        //-------------------------------------------------------

        public async Task ClaimTicketAsync(int ticketId, int technicianId)
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
        }

        //-------------------------------------------------------
        // Escalate Ticket
        //-------------------------------------------------------

        public async Task EscalateTicketAsync(int ticketId, string escalationReason)
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
        }

        //-------------------------------------------------------
        // Resolve Ticket
        //-------------------------------------------------------

        public async Task ResolveTicketAsync(int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
                throw new Exception("Ticket not found.");

            ticket.Status = "Resolved";

            await _ticketRepository.UpdateAsync(ticket);
            await _ticketRepository.SaveChangesAsync();
        }
    }
}
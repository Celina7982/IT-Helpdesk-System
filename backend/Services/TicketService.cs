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

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            return await _ticketRepository.GetAllAsync();
        }

        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            return await _ticketRepository.GetByIdAsync(id);
        }

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

        public async Task AssignTicketAsync(int ticketId, int assignedToUserId)
        {
            // Get the ticket from the database
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            
            //check ticket exists
            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }

            // Assign the senior technician
            ticket.AssignedToUserId = assignedToUserId;

            // Since this is only used for escalated tickets = admin only assigns esculated tickets
            // assigning it means work starts immediately.
            ticket.Status = "In Progress";

            // Ticket is no longer awaiting admin attention =  Ticket has now been accepted back into the workflow.
            ticket.IsEscalated = false;
            ticket.EscalationReason = null;

            // Save the changes in DB
            await _ticketRepository.UpdateAsync(ticket);
            await _ticketRepository.SaveChangesAsync();
        }

        public async Task ClaimTicketAsync(int ticketId, int technicianId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }

            if (ticket.AssignedToUserId != null)
            {
                throw new Exception("Ticket has already been assigned.");
            }

            ticket.AssignedToUserId = technicianId;
            ticket.Status = "In Progress";

            await _ticketRepository.UpdateAsync(ticket);
            await _ticketRepository.SaveChangesAsync();
        }

        public async Task EscalateTicketAsync(int ticketId, string escalationReason)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }

            ticket.Status = "Escalated";
            ticket.IsEscalated = true;
            ticket.EscalationReason = escalationReason;

            // Remove the technician assignment.
            ticket.AssignedToUserId = null;

            await _ticketRepository.UpdateAsync(ticket);
            await _ticketRepository.SaveChangesAsync();
        }

        public async Task ResolveTicketAsync(int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }

            ticket.Status = "Resolved";

            await _ticketRepository.UpdateAsync(ticket);
            await _ticketRepository.SaveChangesAsync();
        }

        public async Task DeleteTicketAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket != null)
            {
                await _ticketRepository.DeleteAsync(ticket);
                await _ticketRepository.SaveChangesAsync();
            }
        }

    }
}
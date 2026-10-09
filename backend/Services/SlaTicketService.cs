using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;

namespace IThelpdesk.Services
{
    public class SlaTicketService : ISlaTicketService
    {
        private readonly ISlaTicketRepository _slaTicketRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IJobCardRepository _jobCardRepository;

        public SlaTicketService(
            ISlaTicketRepository slaTicketRepository,
            ITicketRepository ticketRepository,
            IJobCardRepository jobCardRepository)
        {
            _slaTicketRepository = slaTicketRepository;
            _ticketRepository = ticketRepository;
            _jobCardRepository = jobCardRepository;
        }


        //--------------------------------------------------
        // Create SLA Report From Resolved Ticket
        //--------------------------------------------------

        public async Task<SlaTicket> CreateFromTicketAsync(
            int ticketId,
            int performedByUserId)
        {
            //--------------------------------------------------
            // 1. Get Ticket
            //--------------------------------------------------

            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }


            //--------------------------------------------------
            // 2. Ticket must be Resolved
            //--------------------------------------------------

            if (!string.Equals(
                    ticket.Status,
                    "Resolved",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "An SLA Report can only be created after the ticket has been resolved.");
            }


            //--------------------------------------------------
            // 3. Return existing SLA if already created
            //--------------------------------------------------

            var existingSla =
                await _slaTicketRepository.GetByTicketIdAsync(ticketId);

            if (existingSla != null)
            {
                return existingSla;
            }


            //--------------------------------------------------
            // 4. Ticket must NOT already have a Job Card
            //--------------------------------------------------

            var existingJobCard =
                await _jobCardRepository.GetByTicketIdAsync(ticketId);

            if (existingJobCard != null)
            {
                throw new Exception(
                    $"Ticket #{ticketId} already has Job Card {existingJobCard.JobNumber}. " +
                    "An SLA Report cannot also be created for this ticket.");
            }


            //--------------------------------------------------
            // 5. Generate SLA Number
            //--------------------------------------------------

            var latest =
                await _slaTicketRepository.GetLatestSlaTicketAsync();

            int nextNumber = 1;

            if (latest != null &&
                !string.IsNullOrWhiteSpace(latest.SlaNumber))
            {
                var parts = latest.SlaNumber.Split('-');

                if (parts.Length == 3 &&
                    int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            string slaNumber =
                $"SLA-{DateTime.Now.Year}-{nextNumber:D6}";


            //--------------------------------------------------
            // 6. Safety Check For SLA Number
            //--------------------------------------------------

            while (await _slaTicketRepository
                       .SlaNumberExistsAsync(slaNumber))
            {
                nextNumber++;

                slaNumber =
                    $"SLA-{DateTime.Now.Year}-{nextNumber:D6}";
            }


            //--------------------------------------------------
            // 7. Get Customer Email
            //--------------------------------------------------

            string customerEmail = string.Empty;

            var customer =
                await _ticketRepository.GetUserByIdAsync(ticket.UserId);

            if (customer != null)
            {
                customerEmail = customer.Email ?? string.Empty;
            }


            //--------------------------------------------------
            // 8. Create SLA Report
            //--------------------------------------------------

            var slaTicket = new SlaTicket
            {
                SlaNumber = slaNumber,

                TicketId = ticket.TicketId,

                TechnicianId = ticket.AssignedToUserId,

                CompanyName = ticket.CompanyName ?? string.Empty,

                CustomerName = ticket.CustomerName,

                CustomerEmail = customerEmail,

                Issue = ticket.Description,

                WorkPerformed = string.Empty,

                ResolutionNotes = string.Empty,

                Status = "Open",

                DateCreated = DateTime.UtcNow,

                DateCompleted = null,

                CreatedByUserId = performedByUserId,

                EmailedDate = null,

                RecipientEmail = string.Empty,

                EmailedByUserId = null
            };


            //--------------------------------------------------
            // 9. Save SLA Report
            //--------------------------------------------------

            await _slaTicketRepository.AddAsync(slaTicket);

            return slaTicket;
        }


        //--------------------------------------------------
        // Get All
        //--------------------------------------------------

        public async Task<IEnumerable<SlaTicket>> GetAllAsync()
        {
            return await _slaTicketRepository.GetAllAsync();
        }


        //--------------------------------------------------
        // Get By Id
        //--------------------------------------------------

        public async Task<SlaTicket?> GetByIdAsync(int id)
        {
            return await _slaTicketRepository.GetByIdAsync(id);
        }


        //--------------------------------------------------
        // Get By Ticket Id
        //--------------------------------------------------

        public async Task<SlaTicket?> GetByTicketIdAsync(int ticketId)
        {
            return await _slaTicketRepository.GetByTicketIdAsync(ticketId);
        }


        //--------------------------------------------------
        // Update SLA Report
        //--------------------------------------------------

        public async Task<SlaTicket> UpdateAsync(
            int id,
            string workPerformed,
            string resolutionNotes,
            string status,
            int performedByUserId)
        {
            var slaTicket =
                await _slaTicketRepository.GetByIdAsync(id);

            if (slaTicket == null)
            {
                throw new Exception("SLA Report not found.");
            }


            //--------------------------------------------------
            // Update Work Information
            //--------------------------------------------------

            slaTicket.WorkPerformed =
                workPerformed ?? string.Empty;

            slaTicket.ResolutionNotes =
                resolutionNotes ?? string.Empty;


            //--------------------------------------------------
            // Update Status
            //--------------------------------------------------

            var previousStatus = slaTicket.Status;

            slaTicket.Status =
                string.IsNullOrWhiteSpace(status)
                    ? slaTicket.Status
                    : status;


            //--------------------------------------------------
            // Completion Date
            //--------------------------------------------------

            if (string.Equals(
                    slaTicket.Status,
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
            {
                if (!string.Equals(
                        previousStatus,
                        "Completed",
                        StringComparison.OrdinalIgnoreCase))
                {
                    slaTicket.DateCompleted = DateTime.UtcNow;
                }
            }
            else
            {
                slaTicket.DateCompleted = null;
            }


            //--------------------------------------------------
            // Save
            //--------------------------------------------------

            await _slaTicketRepository.UpdateAsync(slaTicket);

            return slaTicket;
        }


        //--------------------------------------------------
        // Delete SLA Report
        //--------------------------------------------------

        public async Task DeleteAsync(
            int id,
            int performedByUserId)
        {
            var slaTicket =
                await _slaTicketRepository.GetByIdAsync(id);

            if (slaTicket == null)
            {
                throw new Exception("SLA Report not found.");
            }

            await _slaTicketRepository.DeleteAsync(slaTicket);
        }
    }
}
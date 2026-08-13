using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;
using IThelpdesk.DTOs.Ticket;
using System.Security.Claims;

namespace IThelpdesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // ======================================================
        // ADMIN & TECHNICIAN
        // ======================================================

        // GET: api/Ticket
        [Authorize(Roles = "Admin,Technician")]
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        // GET: api/Ticket/available
        [Authorize(Roles = "Technician,Admin")]
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableTickets()
        {
            var tickets = await _ticketService.GetAvailableTicketsAsync();

            return Ok(tickets);
        }

        // GET: api/Ticket/my
        // Returns tickets assigned to the logged-in technician/admin
        [Authorize(Roles = "Technician,Admin")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTickets()
        {
            var technicianId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var tickets =
                await _ticketService.GetMyTicketsAsync(technicianId);

            return Ok(tickets);
        }


        // ======================================================
        // CLIENT
        // ======================================================

        // GET: api/Ticket/mytickets
        // Returns tickets created by the logged-in client
        [Authorize]
        [HttpGet("mytickets")]
        public async Task<IActionResult> GetMyCreatedTickets()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var tickets = await _ticketService.GetMyTicketsByUserAsync(userId);

            return Ok(tickets);
        }

        // ======================================================
        // GET TICKET BY ID
        // ======================================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var ticket = await _ticketService.GetTicketDetailsAsync(id);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        // ======================================================
        // CREATE TICKET
        // ======================================================

        // POST: api/Ticket
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var ticket = new Ticket
            {
                Subject = request.Subject,
                Description = request.Description,
                CustomerName = request.CustomerName,
                CompanyName = request.CompanyName,
                Category = request.Category,
                Priority = request.Priority,

                UserId = userId,
                Status = "Open",
                AssignedToUserId = null,
                IsEscalated = false,
                EscalationReason = null,
                CreatedDate = DateTime.UtcNow
            };

            await _ticketService.CreateTicketAsync(ticket);

            return CreatedAtAction(
                nameof(GetTicket),
                new { id = ticket.TicketId },
                ticket);
        }

        // ======================================================
        // UPDATE TICKET
        // ======================================================

        // PUT: api/Ticket/5
        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] Ticket ticket)
        {
            if (id != ticket.TicketId)
                return BadRequest();

            await _ticketService.UpdateTicketAsync(ticket);

            return NoContent();
        }

        // ======================================================
        // ASSIGN TICKET
        // ======================================================

        // PUT: api/Ticket/5/assign
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignTicket(
            int id,
            [FromBody] AssignTicketRequest request)
        {
            await _ticketService.AssignTicketAsync(
                id,
                request.AssignedToUserId);

            return NoContent();
        }

        // ======================================================
        // CLAIM TICKET
        // ======================================================

        // PUT: api/Ticket/5/claim
        [Authorize(Roles = "Technician,Admin")]
        [HttpPut("{id}/claim")]
        public async Task<IActionResult> ClaimTicket(int id)
        {
            var technicianId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            await _ticketService.ClaimTicketAsync(
                id,
                technicianId);

            return NoContent();
        }

        // ======================================================
        // ESCALATE
        // ======================================================

        // PUT: api/Ticket/5/escalate
        [Authorize(Roles = "Technician,Admin")]
        [HttpPut("{id}/escalate")]
        public async Task<IActionResult> EscalateTicket(
            int id,
            [FromBody] EscalateTicketRequest request)
        {
            await _ticketService.EscalateTicketAsync(
                id,
                request.EscalationReason);

            return NoContent();
        }

        // ======================================================
        // RESOLVE
        // ======================================================

        // PUT: api/Ticket/5/resolve

        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveTicket(int id)
        {
            var resolvedByUserId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );
            await _ticketService.ResolveTicketAsync(
                    id,
                    resolvedByUserId
                );
            return NoContent();
        }

        // ======================================================
        // ARCHIVE
        // ======================================================

        // PUT: api/Ticket/5/archive
        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/archive")]
        public async Task<IActionResult> ArchiveTicket(int id)
        {
            try
            {
                await _ticketService.ArchiveTicketAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Ticket not found.")
                    return NotFound(new { message = ex.Message });

                return BadRequest(new { message = ex.Message });
            }
        }


        // ======================================================
        // DELETE
        // ======================================================

        // DELETE: api/Ticket/5
        [Authorize(Roles = "Admin,Technician")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            await _ticketService.DeleteTicketAsync(id);

            return NoContent();
        }

        // ======================================================
        // ADMIN - ESCALATED TICKETS
        // ======================================================

        // GET: api/Ticket/escalated
        [Authorize(Roles = "Admin")]
        [HttpGet("escalated")]
        public async Task<IActionResult> GetEscalatedTickets()
        {
            var tickets = await _ticketService.GetEscalatedTicketsAsync();

            return Ok(tickets);
        }

        // ======================================================
        // ADMIN - ARCHIVED TICKETS
        // ======================================================

        // GET: api/Ticket/archived
        [Authorize(Roles = "Admin")]
        [HttpGet("archived")]
        public async Task<IActionResult> GetArchivedTickets()
        {
            var tickets = await _ticketService.GetArchivedTicketsAsync();

            return Ok(tickets);
        }

    }

}
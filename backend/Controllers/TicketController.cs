using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;
using IThelpdesk.DTOs.Tickets;
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

        // GET: api/Ticket
        // Admins and Technicians can view all tickets
        [Authorize(Roles = "Admin,Technician")]
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        // GET: api/Ticket/5
        // Any authenticated user can view a ticket by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }


        // POST: api/Ticket
        // Any authenticated user can create a ticket
        // POST: api/Ticket
        // Any authenticated user can create a ticket
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the logged-in user's ID
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            // Create the Ticket entity
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



        // PUT: api/Ticket/5
        // Admins and Technicians can update tickets
        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] Ticket ticket)
        {
            if (id != ticket.TicketId)
                return BadRequest();

            await _ticketService.UpdateTicketAsync(ticket);

            return NoContent();
        }

        // PUT: api/Ticket/5/assign
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignTicket(int id, [FromBody] AssignTicketRequest request)
        {
            await _ticketService.AssignTicketAsync(id, request.AssignedToUserId);

            return NoContent();
        }

        // PUT: api/Ticket/5/claim
        [Authorize(Roles = "Technician,Admin")]
        [HttpPut("{id}/claim")]
        public async Task<IActionResult> ClaimTicket(int id)
        {
            var technicianId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            await _ticketService.ClaimTicketAsync(id, technicianId);

            return NoContent();
        }

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

        // PUT: api/Ticket/5/resolve
        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveTicket(int id)
        {
            await _ticketService.ResolveTicketAsync(id);

            return NoContent();
        }


        // DELETE: api/Ticket/5
        // Admins and Technicians can delete tickets
        [Authorize(Roles = "Admin,Technician")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            await _ticketService.DeleteTicketAsync(id);

            return NoContent();
        }

        
    }
}
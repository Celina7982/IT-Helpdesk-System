using Microsoft.AspNetCore.Mvc;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;

namespace IThelpdesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: api/Ticket
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        // GET: api/Ticket/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        // POST: api/Ticket
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] Ticket ticket)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _ticketService.CreateTicketAsync(ticket);

            return CreatedAtAction(
                nameof(GetTicket),
                new { id = ticket.TicketId },
                ticket);
        }

        // PUT: api/Ticket/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] Ticket ticket)
        {
            if (id != ticket.TicketId)
                return BadRequest();

            await _ticketService.UpdateTicketAsync(ticket);

            return NoContent();
        }

        // DELETE: api/Ticket/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            await _ticketService.DeleteTicketAsync(id);

            return NoContent();
        }
    }
}

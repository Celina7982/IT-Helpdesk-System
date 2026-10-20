
using IThelpdesk.DTOs;
using IThelpdesk.DTOs.Ticket;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
        // CONCURRENCY ERROR RESPONSE
        // ======================================================

        private IActionResult ConcurrencyConflict()
        {
            return Conflict(new
            {
                message = "This ticket was modified by another user. Please refresh and try again."
            });
        }

        // ======================================================
        // DUPLICATE ASSIGNMENT ERROR RESPONSE
        // ======================================================

        private IActionResult DuplicateAssignmentConflict()
        {
            return Conflict(new
            {
                message = "This assignment already exists or was created by another request. Please refresh and try again."
            });
        }

        // ======================================================
        // SQL SERVER UNIQUE CONSTRAINT CHECK
        // ======================================================

        private static bool IsUniqueConstraintViolation(
            DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlException
                && (sqlException.Number == 2601 ||
                    sqlException.Number == 2627);
        }

        // ======================================================
        // ADMIN & TECHNICIAN
        // ======================================================

        // GET: api/Ticket
        [Authorize(Roles = "Admin,Technician")]
        [HttpGet]
        public async Task<IActionResult> GetAllTickets(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var tickets = await _ticketService.GetAllTicketsAsync(
                pageNumber,
                pageSize);

            return Ok(tickets);
        }

        // GET: api/Ticket/available
        [Authorize(Roles = "Technician,Admin")]
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableTickets()
        {
            var tickets =
                await _ticketService.GetAvailableTicketsAsync();

            return Ok(tickets);
        }

        // GET: api/Ticket/my
        [Authorize(Roles = "Technician,Admin")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTickets(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var technicianId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var tickets = await _ticketService.GetMyTicketsAsync(
                technicianId,
                pageNumber,
                pageSize);

            return Ok(tickets);
        }

        // ======================================================
        // CLIENT
        // ======================================================

        // GET: api/Ticket/mytickets
        [Authorize]
        [HttpGet("mytickets")]
        public async Task<IActionResult> GetMyCreatedTickets()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var tickets =
                await _ticketService.GetMyTicketsByUserAsync(userId);

            return Ok(tickets);
        }

        // ======================================================
        // GET TICKET BY ID
        // ======================================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var ticket =
                await _ticketService.GetTicketDetailsAsync(id);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        // ======================================================
        // CREATE TICKET
        // ======================================================

        [HttpPost]
        public async Task<IActionResult> CreateTicket(
            [FromBody] CreateTicketDto request)
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

        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(
            int id,
            [FromBody] Ticket ticket)
        {
            if (id != ticket.TicketId)
                return BadRequest();

            try
            {
                await _ticketService.UpdateTicketAsync(ticket);

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyConflict();
            }
        }

        // ======================================================
        // ASSIGN TICKET
        // Admin assigns or changes the primary assignee
        // ======================================================

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/assign")]
        public async Task<IActionResult> AssignTicket(
            int id,
            [FromBody] AssignTicketRequest request)
        {
            try
            {
                var adminId = int.Parse(
                    User.FindFirst(ClaimTypes.NameIdentifier)!.Value
                );

                await _ticketService.AssignTicketAsync(
                    id,
                    request.AssignedToUserId,
                    adminId
                );

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyConflict();
            }
            catch (DbUpdateException ex)
                when (IsUniqueConstraintViolation(ex))
            {
                return DuplicateAssignmentConflict();
            }
        }

        // ======================================================
        // GET TICKET ASSIGNEES
        // ======================================================

        [Authorize(Roles = "Admin,Technician")]
        [HttpGet("{id}/assignees")]
        public async Task<IActionResult> GetTicketAssignees(int id)
        {
            try
            {
                var assignees =
                    await _ticketService.GetTicketAssigneesAsync(id);

                return Ok(assignees);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // ======================================================
        // ADD TICKET ASSIGNEE
        // Admin only
        // ======================================================

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/assignees")]
        public async Task<IActionResult> AddTicketAssignee(
            int id,
            [FromBody] AddTicketAssigneeRequest request)
        {
            try
            {
                var adminId = int.Parse(
                    User.FindFirst(ClaimTypes.NameIdentifier)!.Value
                );

                await _ticketService.AddTicketAssigneeAsync(
                    id,
                    request.UserId,
                    adminId);

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyConflict();
            }
            catch (DbUpdateException ex)
                when (IsUniqueConstraintViolation(ex))
            {
                return DuplicateAssignmentConflict();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // ======================================================
        // REMOVE TICKET ASSIGNEE
        // Admin only
        // ======================================================

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/assignees/{userId}")]
        public async Task<IActionResult> RemoveTicketAssignee(
            int id,
            int userId)
        {
            try
            {
                var adminId = int.Parse(
                    User.FindFirst(ClaimTypes.NameIdentifier)!.Value
                );

                await _ticketService.RemoveTicketAssigneeAsync(
                    id,
                    userId,
                    adminId);

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyConflict();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // ======================================================
        // CLAIM TICKET
        // ======================================================

        [Authorize(Roles = "Technician,Admin")]
        [HttpPut("{id}/claim")]
        public async Task<IActionResult> ClaimTicket(int id)
        {
            try
            {
                var technicianId = int.Parse(
                    User.FindFirst(ClaimTypes.NameIdentifier)!.Value
                );

                await _ticketService.ClaimTicketAsync(
                    id,
                    technicianId);

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyConflict();
            }
            catch (DbUpdateException ex)
                when (IsUniqueConstraintViolation(ex))
            {
                return DuplicateAssignmentConflict();
            }
        }

        // ======================================================
        // ESCALATE TICKET
        // ======================================================

        [Authorize(Roles = "Technician,Admin")]
        [HttpPut("{id}/escalate")]
        public async Task<IActionResult> EscalateTicket(
            int id,
            [FromBody] EscalateTicketRequest request)
        {
            try
            {
                await _ticketService.EscalateTicketAsync(
                    id,
                    request.EscalationReason);

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyConflict();
            }
        }

        // ======================================================
        // RESOLVE TICKET
        // ======================================================

        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> ResolveTicket(int id)
        {
            try
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
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyConflict();
            }
        }

        // ======================================================
        // ARCHIVE TICKET
        // ======================================================

        [Authorize(Roles = "Admin,Technician")]
        [HttpPut("{id}/archive")]
        public async Task<IActionResult> ArchiveTicket(int id)
        {
            try
            {
                await _ticketService.ArchiveTicketAsync(id);

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyConflict();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Ticket not found.")
                {
                    return NotFound(new
                    {
                        message = ex.Message
                    });
                }

                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // ======================================================
        // DELETE TICKET
        // ======================================================

        [Authorize(Roles = "Admin,Technician")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            try
            {
                await _ticketService.DeleteTicketAsync(id);

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return ConcurrencyConflict();
            }
        }

        // ======================================================
        // ADMIN - ESCALATED TICKETS
        // ======================================================

        [Authorize(Roles = "Admin")]
        [HttpGet("escalated")]
        public async Task<IActionResult> GetEscalatedTickets(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var tickets =
                await _ticketService.GetEscalatedTicketsAsync(
                    pageNumber,
                    pageSize);

            return Ok(tickets);
        }

        // ======================================================
        // ADMIN - ARCHIVED TICKETS
        // ======================================================

        [Authorize(Roles = "Admin")]
        [HttpGet("archived")]
        public async Task<IActionResult> GetArchivedTickets(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var tickets =
                await _ticketService.GetArchivedTicketsAsync(
                    pageNumber,
                    pageSize);

            return Ok(tickets);
        }
    }
}

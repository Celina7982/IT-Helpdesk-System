    using IThelpdesk.Data;
    using IThelpdesk.DTOs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Claims;
    using IThelpdesk.Data; // Adjust to your DbContext namespace
    using IThelpdesk.DTOs;
    using IThelpdesk.Models;

    namespace IThelpdesk.Controllers
    {
        [ApiController]
        [Route("api/tickets/{ticketId}/comments")]
        [Authorize] // Requires valid JWT Token
        public class TicketCommentsController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public TicketCommentsController(ApplicationDbContext context)
            {
                _context = context;
            }

            // GET: api/tickets/{ticketId}/comments
            [HttpGet]
            public async Task<IActionResult> GetComments(int ticketId)
            {
                var ticketExists = await _context.Tickets.AnyAsync(t => t.TicketId == ticketId);
                if (!ticketExists)
                {
                    return NotFound(new { message = "Ticket not found." });
                }

                var comments = await _context.TicketComments
                    .Where(c => c.TicketId == ticketId)
                    .OrderBy(c => c.CreatedDate)
                    .Select(c => new CommentResponseDto
                    {
                        CommentId = c.CommentId,
                        TicketId = c.TicketId,
                        AuthorName = c.AuthorName,
                        Message = c.Message,
                        CreatedDate = c.CreatedDate
                    })
                    .ToListAsync();

                return Ok(comments);
            }

            // POST: api/tickets/{ticketId}/comments
            [HttpPost]
            public async Task<IActionResult> AddComment(int ticketId, [FromBody] CreateCommentDto dto)
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Message))
                {
                    return BadRequest(new { message = "Comment message cannot be empty." });
                }

                // 1. Find the ticket
                var ticket = await _context.Tickets.FindAsync(ticketId);
                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket not found." });
                }

            if (string.Equals(ticket.Status, "Resolved", StringComparison.OrdinalIgnoreCase) ||
string.Equals(ticket.Status, "Closed", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Comments are disabled for resolved or closed tickets." });
            }

            // 3. Extract the logged-in user's name from JWT claims
            var authorName = User.FindFirstValue(ClaimTypes.Name)
                              ?? User.FindFirstValue(ClaimTypes.GivenName)
                              ?? "Client";

                // 4. Save Comment
                var comment = new TicketComment
                {
                    TicketId = ticketId,
                    AuthorName = authorName,
                    Message = dto.Message.Trim(),
                    CreatedDate = DateTime.Now
                };

                _context.TicketComments.Add(comment);
                await _context.SaveChangesAsync();

                // 5. Return Created Comment Response
                var response = new CommentResponseDto
                {
                    CommentId = comment.CommentId,
                    TicketId = comment.TicketId,
                    AuthorName = comment.AuthorName,
                    Message = comment.Message,
                    CreatedDate = comment.CreatedDate
                };

                return Ok(response);
            }
        }
    }

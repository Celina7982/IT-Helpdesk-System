using IThelpdesk.Data;
using IThelpdesk.DTOs;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IThelpdesk.Controllers
{
    [ApiController]
    [Route("api/tickets/{ticketId}/comments")]
    [Authorize]
    public class TicketCommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public TicketCommentsController(
            ApplicationDbContext context,
            INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        //-------------------------------------------------------
        // GET: api/tickets/{ticketId}/comments
        //-------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> GetComments(int ticketId)
        {
            var ticketExists = await _context.Tickets
                .AnyAsync(t => t.TicketId == ticketId);

            if (!ticketExists)
            {
                return NotFound(new
                {
                    message = "Ticket not found."
                });
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

        //-------------------------------------------------------
        // POST: api/tickets/{ticketId}/comments
        //-------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> AddComment(
            int ticketId,
            [FromBody] CreateCommentDto dto)
        {
            //---------------------------------------------------
            // Validate comment
            //---------------------------------------------------

            if (dto == null || string.IsNullOrWhiteSpace(dto.Message))
            {
                return BadRequest(new
                {
                    message = "Comment message cannot be empty."
                });
            }

            //---------------------------------------------------
            // Find ticket
            //---------------------------------------------------

            var ticket = await _context.Tickets
                .FindAsync(ticketId);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Ticket not found."
                });
            }

            //---------------------------------------------------
            // Prevent comments on resolved/closed tickets
            //---------------------------------------------------

            if (string.Equals(
                    ticket.Status,
                    "Resolved",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    ticket.Status,
                    "Closed",
                    StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "Comments are disabled for resolved or closed tickets."
                });
            }

            //---------------------------------------------------
            // Get logged-in user's ID from JWT
            //---------------------------------------------------

            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int loggedInUserId))
            {
                return Unauthorized(new
                {
                    message = "Unable to identify logged-in user."
                });
            }

            //---------------------------------------------------
            // Get logged-in user's name
            //---------------------------------------------------

            var authorName =
                User.FindFirstValue(ClaimTypes.Name)
                ?? User.FindFirstValue(ClaimTypes.GivenName)
                ?? "User";

            //---------------------------------------------------
            // Create comment
            //---------------------------------------------------

            var comment = new TicketComment
            {
                TicketId = ticketId,
                AuthorName = authorName,
                Message = dto.Message.Trim(),
                CreatedDate = DateTime.UtcNow
            };

            _context.TicketComments.Add(comment);

            await _context.SaveChangesAsync();

            //---------------------------------------------------
            // Notify Customer
            //---------------------------------------------------
            //
            // If the person adding the comment is NOT the
            // customer who owns the ticket, notify the customer.
            //
            //---------------------------------------------------

            if (ticket.UserId != loggedInUserId)
            {
                await _notificationService.CreateAsync(
                ticket.UserId,
                "New Ticket Update",
                $"There is a new update on your ticket '{ticket.Subject}'.",
                ticket.TicketId);
            }

            //---------------------------------------------------
            // Return Created Comment
            //---------------------------------------------------

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

        //-------------------------------------------------------
        // PUT:
        // api/tickets/{ticketId}/comments/{commentId}
        //-------------------------------------------------------

        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(
            int ticketId,
            int commentId,
            [FromBody] UpdateCommentDto dto)
        {
            //---------------------------------------------------
            // Validate comment
            //---------------------------------------------------

            if (dto == null || string.IsNullOrWhiteSpace(dto.Message))
            {
                return BadRequest(new
                {
                    message = "Comment message cannot be empty."
                });
            }

            //---------------------------------------------------
            // Find comment
            //---------------------------------------------------

            var comment = await _context.TicketComments
                .FirstOrDefaultAsync(c =>
                    c.CommentId == commentId &&
                    c.TicketId == ticketId);

            if (comment == null)
            {
                return NotFound(new
                {
                    message = "Comment not found."
                });
            }

            //---------------------------------------------------
            // Find ticket
            //---------------------------------------------------

            var ticket = await _context.Tickets
                .FindAsync(ticketId);

            if (ticket == null)
            {
                return NotFound(new
                {
                    message = "Ticket not found."
                });
            }

            //---------------------------------------------------
            // Prevent editing comments on resolved/closed tickets
            //---------------------------------------------------

            if (string.Equals(
                    ticket.Status,
                    "Resolved",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    ticket.Status,
                    "Closed",
                    StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    message = "Comments cannot be edited on resolved or closed tickets."
                });
            }

            //---------------------------------------------------
            // Update comment
            //---------------------------------------------------

            comment.Message = dto.Message.Trim();

            await _context.SaveChangesAsync();

            //---------------------------------------------------
            // Return updated comment
            //---------------------------------------------------

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
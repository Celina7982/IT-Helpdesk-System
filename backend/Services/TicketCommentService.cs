using IThelpdesk.DTOs;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;

namespace IThelpdesk.Services
{
    public class TicketCommentService : ITicketCommentService
    {
        private readonly ITicketCommentRepository _commentRepository;
        private readonly INotificationService _notificationService;

        public TicketCommentService(
            ITicketCommentRepository commentRepository,
            INotificationService notificationService)
        {
            _commentRepository = commentRepository;
            _notificationService = notificationService;
        }

        //-------------------------------------------------------
        // Get Comments
        //-------------------------------------------------------

        public async Task<List<CommentResponseDto>> GetCommentsAsync(
     int ticketId,
     string userRole)
        {
            var ticketExists =
                await _commentRepository.TicketExistsAsync(ticketId);

            if (!ticketExists)
                throw new KeyNotFoundException("Ticket not found.");

            var comments =
                await _commentRepository.GetByTicketIdAsync(ticketId);

            //-------------------------------------------------------
            // Hide Internal Notes From Clients
            //-------------------------------------------------------

            var isStaff =
                string.Equals(
                    userRole,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    userRole,
                    "Technician",
                    StringComparison.OrdinalIgnoreCase);

            if (!isStaff)
            {
                comments = comments
                    .Where(c => !c.IsInternal)
                    .ToList();
            }

            return comments
                .Select(c => new CommentResponseDto
                {
                    CommentId = c.CommentId,
                    TicketId = c.TicketId,
                    AuthorUserId = c.AuthorUserId,

                    AuthorName = c.AuthorName,
                    Message = c.Message,
                    IsInternal = c.IsInternal,
                    CreatedDate = c.CreatedDate
                })
                .ToList();
        }

        //-------------------------------------------------------
        // Add Comment
        //-------------------------------------------------------

        public async Task<CommentResponseDto> AddCommentAsync(
     int ticketId,
     CreateCommentDto dto,
     int loggedInUserId,
     string authorName,
     string authorRole)
        {
            //-------------------------------------------------------
            // Validate Comment
            //-------------------------------------------------------

            if (dto == null || string.IsNullOrWhiteSpace(dto.Message))
                throw new ArgumentException(
                    "Comment message cannot be empty.");

            //-------------------------------------------------------
            // Get Ticket
            //-------------------------------------------------------

            var ticket =
                await _commentRepository.GetTicketByIdAsync(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

            //-------------------------------------------------------
            // Prevent Comments On Resolved / Closed Tickets
            //-------------------------------------------------------

            if (string.Equals(
                    ticket.Status,
                    "Resolved",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    ticket.Status,
                    "Closed",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Comments are disabled for resolved or closed tickets.");
            }

            //-------------------------------------------------------
            // Internal Note Permission
            //-------------------------------------------------------

            if (dto.IsInternal &&
                !string.Equals(
                    authorRole,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(
                    authorRole,
                    "Technician",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException(
                    "Only Admins and Technicians can create internal notes.");
            }

            //-------------------------------------------------------
            // Create Comment
            //-------------------------------------------------------

            var comment = new TicketComment
            {
                TicketId = ticketId,
                AuthorUserId = loggedInUserId,
                AuthorName = authorName,
                Message = dto.Message.Trim(),
                IsInternal = dto.IsInternal,
                CreatedDate = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();

            //-------------------------------------------------------
            // Notify Customer
            //-------------------------------------------------------

            if (!comment.IsInternal &&
    ticket.UserId != loggedInUserId)
            {
                await _notificationService.CreateAsync(
                    ticket.UserId,
                    "New Ticket Update",
                    $"There is a new update on your ticket '{ticket.Subject}'.",
                    ticket.TicketId);
            }

            //-------------------------------------------------------
            // Notify Assigned Technician
            //-------------------------------------------------------

            if (ticket.AssignedToUserId.HasValue &&
                ticket.AssignedToUserId.Value != loggedInUserId &&
                (string.Equals(
                     authorRole,
                     "Client",
                     StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(
                     authorRole,
                     "Admin",
                     StringComparison.OrdinalIgnoreCase)))
            {
                if (comment.IsInternal)
                {
                    await _notificationService.CreateAsync(
                        ticket.AssignedToUserId.Value,
                        "New Internal Note",
                        $"A new internal note was added to Ticket #{ticket.TicketId}.",
                        ticket.TicketId);
                }
                else
                {
                    await _notificationService.CreateAsync(
                        ticket.AssignedToUserId.Value,
                        "New Ticket Comment",
                        $"A new comment was added to Ticket #{ticket.TicketId}.",
                        ticket.TicketId);
                }
            }

            //-------------------------------------------------------
            // Return Comment
            //-------------------------------------------------------

            return new CommentResponseDto
            {
                CommentId = comment.CommentId,
                TicketId = comment.TicketId,
                AuthorUserId = comment.AuthorUserId,
                AuthorName = comment.AuthorName,
                Message = comment.Message,
                IsInternal = comment.IsInternal,
                CreatedDate = comment.CreatedDate
            };
        }

        //-------------------------------------------------------
        // Update Comment
        //-------------------------------------------------------

        public async Task<CommentResponseDto> UpdateCommentAsync(
     int ticketId,
     int commentId,
     UpdateCommentDto dto,
     int loggedInUserId,
     string userRole)
        {
            //-------------------------------------------------------
            // Validate Comment
            //-------------------------------------------------------

            if (dto == null || string.IsNullOrWhiteSpace(dto.Message))
                throw new ArgumentException(
                    "Comment message cannot be empty.");

            //-------------------------------------------------------
            // Get Comment
            //-------------------------------------------------------

            var comment =
                await _commentRepository.GetByIdAsync(
                    ticketId,
                    commentId);

            if (comment == null)
                throw new KeyNotFoundException("Comment not found.");

            //-------------------------------------------------------
            // Get Ticket
            //-------------------------------------------------------

            var ticket =
                await _commentRepository.GetTicketByIdAsync(ticketId);

            if (ticket == null)
                throw new KeyNotFoundException("Ticket not found.");

            //-------------------------------------------------------
            // Check Edit Permission
            //-------------------------------------------------------

            var isAdmin =
                string.Equals(
                    userRole,
                    "Admin",
                    StringComparison.OrdinalIgnoreCase);

            // Admin can edit any comment.
            // Other users can only edit comments they created.
            if (!isAdmin &&
                comment.AuthorUserId != loggedInUserId)
            {
                throw new UnauthorizedAccessException(
                    "You can only edit your own comments.");
            }

            //-------------------------------------------------------
            // Prevent Editing On Resolved / Closed Tickets
            //-------------------------------------------------------

            if (string.Equals(
                    ticket.Status,
                    "Resolved",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    ticket.Status,
                    "Closed",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Comments cannot be edited on resolved or closed tickets.");
            }

            //-------------------------------------------------------
            // Update Comment
            //-------------------------------------------------------

            comment.Message = dto.Message.Trim();

            await _commentRepository.SaveChangesAsync();

            //-------------------------------------------------------
            // Return Updated Comment
            //-------------------------------------------------------

            return new CommentResponseDto
            {
                CommentId = comment.CommentId,
                TicketId = comment.TicketId,
                AuthorUserId = comment.AuthorUserId,
                AuthorName = comment.AuthorName,
                Message = comment.Message,
                IsInternal = comment.IsInternal,
                CreatedDate = comment.CreatedDate
            };
        }
    }
}
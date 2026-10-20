using IThelpdesk.DTOs;
using IThelpdesk.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IThelpdesk.Controllers
{
    [ApiController]
    [Route("api/tickets/{ticketId}/comments")]
    [Authorize]
    public class TicketCommentsController : ControllerBase
    {
        private readonly ITicketCommentService _ticketCommentService;

        public TicketCommentsController(
            ITicketCommentService ticketCommentService)
        {
            _ticketCommentService = ticketCommentService;
        }

        //-------------------------------------------------------
        // GET: api/tickets/{ticketId}/comments
        //-------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetComments(int ticketId)
        {
            //-------------------------------------------------------
            // Get Logged-In User's Role
            //-------------------------------------------------------

            var userRole =
                User.FindFirstValue(ClaimTypes.Role)
                ?? "";

            //-------------------------------------------------------
            // Get Comments
            //-------------------------------------------------------

            try
            {
                var comments =
                    await _ticketCommentService.GetCommentsAsync(
                        ticketId,
                        userRole);

                return Ok(comments);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
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
            // Get logged-in user's ID
            //---------------------------------------------------

            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                    userIdClaim,
                    out int loggedInUserId))
            {
                return Unauthorized(new
                {
                    message =
                        "Unable to identify logged-in user."
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
            // Get logged-in user's role
            //---------------------------------------------------

            var authorRole =
                User.FindFirstValue(ClaimTypes.Role)
                ?? "";

            //---------------------------------------------------
            // Add Comment
            //---------------------------------------------------

            try
            {
                var comment =
                    await _ticketCommentService.AddCommentAsync(
                        ticketId,
                        dto,
                        loggedInUserId,
                        authorName,
                        authorRole);

                return Ok(comment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
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
            // Get logged-in user's ID
            //---------------------------------------------------

            var userIdClaim =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                    userIdClaim,
                    out int loggedInUserId))
            {
                return Unauthorized(new
                {
                    message =
                        "Unable to identify logged-in user."
                });
            }

            //---------------------------------------------------
            // Get logged-in user's role
            //---------------------------------------------------

            var userRole =
                User.FindFirstValue(ClaimTypes.Role)
                ?? "";

            //---------------------------------------------------
            // Update Comment
            //---------------------------------------------------

            try
            {
                var comment =
                    await _ticketCommentService.UpdateCommentAsync(
                        ticketId,
                        commentId,
                        dto,
                        loggedInUserId,
                        userRole);

                return Ok(comment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new
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
    }
}
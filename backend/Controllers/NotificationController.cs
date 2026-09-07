using IThelpdesk.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IThelpdesk.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        //--------------------------------------------------
        // Get My Notifications
        //--------------------------------------------------

        // GET: api/notifications
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }

            var notifications =
                await _notificationService.GetByUserIdAsync(userId);

            return Ok(notifications);
        }

        //--------------------------------------------------
        // Get My Unread Notifications
        //--------------------------------------------------

        // GET: api/notifications/unread
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }

            var notifications =
                await _notificationService.GetUnreadByUserIdAsync(userId);

            return Ok(notifications);
        }

        //--------------------------------------------------
        // Mark Notification As Read
        //--------------------------------------------------

        // PUT: api/notifications/{notificationId}/read
        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }

            await _notificationService.MarkAsReadAsync(
                notificationId,
                userId);

            return Ok(new
            {
                message = "Notification marked as read."
            });
        }

        //--------------------------------------------------
        // Delete Notification
        //--------------------------------------------------

        // DELETE: api/notifications/{notificationId}
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(
            int notificationId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }

            await _notificationService.DeleteAsync(
                notificationId,
                userId);

            return Ok(new
            {
                message = "Notification deleted."
            });
        }

       



    }

}
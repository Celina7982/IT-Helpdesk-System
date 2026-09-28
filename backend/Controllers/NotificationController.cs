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

        // GET: api/notifications?pageNumber=1&pageSize=10

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }

            //--------------------------------------------------
            // Validate pagination
            //--------------------------------------------------

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            //--------------------------------------------------
            // Prevent excessively large requests
            //--------------------------------------------------

            if (pageSize > 50)
            {
                pageSize = 50;
            }

            //--------------------------------------------------
            // Get paginated notifications
            //--------------------------------------------------

            var notifications =
                await _notificationService.GetPagedByUserIdAsync(
                    userId,
                    pageNumber,
                    pageSize);

            return Ok(notifications);
        }



        //--------------------------------------------------
        // Get My Unread Notification Count
        //--------------------------------------------------

        // GET: api/notifications/unread/count

        [HttpGet("unread/count")]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user identity."
                });
            }
            
            var unreadCount =
                await _notificationService
                    .GetUnreadCountByUserIdAsync(userId);

            return Ok(new
            {
                count = unreadCount
            });
        }
        //--------------------------------------------------
        // Mark Notification As Read
        //--------------------------------------------------

        // PUT: api/notifications/{notificationId}/read

        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(
            int notificationId)
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

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
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

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
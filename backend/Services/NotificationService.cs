using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;

namespace IThelpdesk.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(
            INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        //--------------------------------------------------
        // Create Notification
        //--------------------------------------------------

        public async Task CreateAsync(
    int userId,
    string title,
    string message,
    int? ticketId = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                TicketId = ticketId,
                IsRead = false,
                DateCreated = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();
        }


        //--------------------------------------------------
        // Get User Notifications
        //--------------------------------------------------

        public async Task<List<Notification>> GetByUserIdAsync(
            int userId)
        {
            return await _notificationRepository
                .GetByUserIdAsync(userId);
        }

        //--------------------------------------------------
        // Get Unread Notifications
        //--------------------------------------------------

        public async Task<List<Notification>> GetUnreadByUserIdAsync(
            int userId)
        {
            return await _notificationRepository
                .GetUnreadByUserIdAsync(userId);
        }

        //--------------------------------------------------
        // Mark Notification As Read
        //--------------------------------------------------

        public async Task MarkAsReadAsync(
            int notificationId,
            int userId)
        {
            var notification =
                await _notificationRepository
                    .GetByIdAsync(notificationId);

            if (notification == null)
                throw new Exception("Notification not found.");

            //--------------------------------------------------
            // Security:
            // Notification must belong to logged-in user
            //--------------------------------------------------

            if (notification.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "You cannot modify this notification.");
            }

            notification.IsRead = true;

            await _notificationRepository.SaveChangesAsync();
        }

        //--------------------------------------------------
        // Delete Notification
        //--------------------------------------------------

        public async Task DeleteAsync(
            int notificationId,
            int userId)
        {
            var notification =
                await _notificationRepository
                    .GetByIdAsync(notificationId);

            if (notification == null)
                throw new Exception("Notification not found.");

            //--------------------------------------------------
            // Security:
            // Notification must belong to logged-in user
            //--------------------------------------------------

            if (notification.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "You cannot delete this notification.");
            }

            await _notificationRepository
                .DeleteAsync(notification);

            await _notificationRepository
                .SaveChangesAsync();
        }
    }
}
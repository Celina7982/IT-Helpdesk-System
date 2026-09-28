using IThelpdesk.DTOs.Common;
using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Services
{
    public interface INotificationService
    {
        //--------------------------------------------------
        // Create Notification
        //--------------------------------------------------

        Task CreateAsync(
            int userId,
            string title,
            string message,
            int? ticketId = null);


        //--------------------------------------------------
        // Get Paginated Notifications For User
        //--------------------------------------------------

        Task<PagedResultDto<NotificationDto>> GetPagedByUserIdAsync(
            int userId,
            int pageNumber,
            int pageSize);


        //--------------------------------------------------
        // Get Unread Notifications
        //--------------------------------------------------

        Task<int> GetUnreadCountByUserIdAsync(int userId);

        //--------------------------------------------------
        // Mark Notification As Read
        //--------------------------------------------------

        Task MarkAsReadAsync(
            int notificationId,
            int userId);


        //--------------------------------------------------
        // Delete Notification
        //--------------------------------------------------

        Task DeleteAsync(
            int notificationId,
            int userId);
    }
}
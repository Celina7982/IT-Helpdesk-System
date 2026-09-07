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
        // Get User Notifications
        //--------------------------------------------------

        Task<List<Notification>> GetByUserIdAsync(
            int userId);

        //--------------------------------------------------
        // Get Unread Notifications
        //--------------------------------------------------

        Task<List<Notification>> GetUnreadByUserIdAsync(
            int userId);

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
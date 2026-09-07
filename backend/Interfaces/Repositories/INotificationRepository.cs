using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        //--------------------------------------------------
        // Add Notification
        //--------------------------------------------------

        Task AddAsync(Notification notification);

        //--------------------------------------------------
        // Get All Notifications For User
        //--------------------------------------------------

        Task<List<Notification>> GetByUserIdAsync(int userId);

        //--------------------------------------------------
        // Get Unread Notifications For User
        //--------------------------------------------------

        Task<List<Notification>> GetUnreadByUserIdAsync(int userId);

        //--------------------------------------------------
        // Get Notification By ID
        //--------------------------------------------------

        Task<Notification?> GetByIdAsync(int notificationId);

        //--------------------------------------------------
        // Delete Notification
        //--------------------------------------------------

        Task DeleteAsync(Notification notification);

        //--------------------------------------------------
        // Save Changes
        //--------------------------------------------------

        Task SaveChangesAsync();
    }
}

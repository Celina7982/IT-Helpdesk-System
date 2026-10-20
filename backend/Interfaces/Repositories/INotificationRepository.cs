using IThelpdesk.Models;
using IThelpdesk.DTOs.Common;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        //--------------------------------------------------
        // Add Notification
        //--------------------------------------------------

        Task AddAsync(Notification notification);

        //--------------------------------------------------
        // Get Paginated Notifications For User
        //--------------------------------------------------
        Task<PagedResultDto<NotificationDto>> GetPagedByUserIdAsync(
            int userId,
            int pageNumber,
            int pageSize);



        //--------------------------------------------------
        // Get Unread Notifications For User
        //--------------------------------------------------

        Task<int> GetUnreadCountByUserIdAsync(int userId);

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

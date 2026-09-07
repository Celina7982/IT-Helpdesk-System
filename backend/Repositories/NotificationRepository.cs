using IThelpdesk.Data;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Models;
using Microsoft.EntityFrameworkCore;

namespace IThelpdesk.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //--------------------------------------------------
        // Add Notification
        //--------------------------------------------------

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        //--------------------------------------------------
        // Get Notifications For User
        //--------------------------------------------------

        public async Task<List<Notification>> GetByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
        }

        //--------------------------------------------------
        // Get Unread Notifications For User
        //--------------------------------------------------

        public async Task<List<Notification>> GetUnreadByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
        }

        //--------------------------------------------------
        // Get Notification By ID
        //--------------------------------------------------

        public async Task<Notification?> GetByIdAsync(int notificationId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.NotificationId == notificationId);
        }

        //--------------------------------------------------
        // Delete Notification
        //--------------------------------------------------

        public async Task DeleteAsync(Notification notification)
        {
            _context.Notifications.Remove(notification);

            await Task.CompletedTask;
        }

        //--------------------------------------------------
        // Save Changes
        //--------------------------------------------------

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
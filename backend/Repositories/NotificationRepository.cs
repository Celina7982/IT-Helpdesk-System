using IThelpdesk.Data;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Models;
using Microsoft.EntityFrameworkCore;
using IThelpdesk.DTOs.Common;

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
        // Get Paginated Notifications For User
        //--------------------------------------------------
        public async Task<PagedResultDto<NotificationDto>> GetPagedByUserIdAsync(
            int userId,
            int pageNumber,
            int pageSize)
        {
            var query = _context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.DateCreated);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    TicketId = n.TicketId,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    DateCreated = n.DateCreated
                })
                .ToListAsync();

            return new PagedResultDto<NotificationDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }



        //--------------------------------------------------
        // Get Unread Notifications For User
        //--------------------------------------------------

        public async Task<int> GetUnreadCountByUserIdAsync(
     int userId
 )
        {
            return await _context.Notifications
                .CountAsync(n =>
                    n.UserId == userId &&
                    !n.IsRead
                );
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
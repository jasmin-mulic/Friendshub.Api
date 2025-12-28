using Friendshub.Application.DTO;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Friendshub.Infrastructure.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly FriendshubDbContext _context;
        public NotificationRepository(FriendshubDbContext context)
        {
            _context = context;
        }
        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task<List<Notification>> GetNotificationsAsync(Guid receiverId, int pageNumber = 1)
        {
            const int pageSize = 10;

            if (pageNumber < 1)
                pageNumber = 1;

            return await _context.Notifications
                .Include(n => n.Sender) 
                .Where(n => n.ReceiverId == receiverId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Notification> GetNotificationAsync(Guid notificationId)
        {
            return await _context.Notifications.FirstOrDefaultAsync(x =>  x.Id == notificationId);
        }

        public void MarkAsRead(Notification notification)
        {
            notification.IsOpened = true;
            _context.Notifications.Update(notification);
        }

        public async Task<Notification> GetNotificationAsNoTrackingAsync(Guid notificationId)
        {
            return await _context.Notifications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == notificationId);
        }

        public async Task<Notification> GetNotificationByPostId(Guid postId)
        {
           return await _context.Notifications.FirstOrDefaultAsync(x => x.EntityId == postId);
        }

        public void DeleteNotification(Notification notification)
        {
            _context.Notifications.Remove(notification);
        }

        public int GetNotificationsTotalCount(Guid userId)
        {
            var count =   _context.Notifications.AsNoTracking().Where(n => n.ReceiverId == userId).Count();
            return count;
        }
    }
}

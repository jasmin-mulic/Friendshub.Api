using Friendshub.Application.DTO;
using Friendshub.Application.Extensions;
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

        public async Task CreateNotification(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task<PageResult<ClientNotificationDto>> GetNotificationsAsync(Guid recieverId, int pageNumber = 1)
        {
            var result = new PageResult<ClientNotificationDto>();
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 10) pageSize = 10;

            var querry =  _context.Notifications
                           .Include(x => x.Sender)
                           .Where(x => x.ReceiverId == recieverId);
            var totalCount = querry.Count();

            var notificationEntities = await querry.OrderByDescending(x => x.CreatedAt).Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).AsNoTracking().ToListAsync();

            var notifications = notificationEntities.Select(notif => new ClientNotificationDto
            {
                Message = notif.Message,
                SenderProfileImageUrl = notif.Sender.ProfileImageUrl,
                CreatedAt = notif.CreatedAt,
                SenderUsername = notif.Sender.Username,
                Id = notif.Id,
            }).OrderByDescending(x =>x.CreatedAt).ToList();

            result.Items = notifications;
            result.TotalCount = totalCount;
            result.PageNumber = pageNumber;
            result.PageSize = pageSize;
            return result;
        }

        public async Task MarkAsRead(Guid id)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id);
            if (notification == null)
                throw new ApplicationException("Notification is deleted.");
            notification.isRead = true;
            _context.Notifications.Update(notification);
        }
    }
}

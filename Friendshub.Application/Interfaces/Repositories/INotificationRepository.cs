using Friendshub.Application.DTO;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Repositories
{
    public interface INotificationRepository
    {
        Task<PageResult<ClientNotificationDto>> GetNotificationsAsync(Guid recieverId, int pageNumber = 1);
        Task AddNotificationAsync(Notification notification);
        void MarkAsRead(Notification notification);
        Task<Notification> GetNotificationAsync(Guid notificationId);
        Task<Notification> GetNotificationAsNoTrackingAsync(Guid notificationId);

    }
}

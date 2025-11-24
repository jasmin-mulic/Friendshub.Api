using Friendshub.Application.DTO;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<PageResult<ClientNotificationDto>> GetNotificationsAsync(Guid recieverId, int pageNumber = 1);
        Task<Notification> CreateNotification(Guid senderId, Guid receiverId, NotificationType type, Guid? entityId = null);
        Task MarkAsRead (Guid notificationId);
    }
}

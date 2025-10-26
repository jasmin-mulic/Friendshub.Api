using Friendshub.Application.DTO;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Repositories
{
    public interface INotificationRepository
    {
        Task<PageResult<ClientNotificationDto>> GetNotificationsAsync(Guid recieverId, int pageNumber = 1);
        Task CreateNotification(Guid senderId, Guid receiverId, NotificationType type, Guid? entityId = null);

        Task MarkAsRead(Guid id);
    }
}

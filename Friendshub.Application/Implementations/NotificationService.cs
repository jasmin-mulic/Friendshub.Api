using Friendshub.Application.DTO;
using Friendshub.Application.Extensions;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Implementations
{
    internal class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }
        public async Task<Notification> CreateNotification(Guid senderId, Guid receiverId, NotificationType type, Guid? entityId = null)
        {
            if (senderId == receiverId)
                return null;
            var sender = await _unitOfWork.UserRepository.GetUserByIdAsNoTracking(senderId);

            if (sender == null)
                return null;
            var reciever = await _unitOfWork.UserRepository.GetUserByIdAsNoTracking(senderId);

            if (reciever == null)
                return null;

            var message = sender.Username.BuildNotificationMessage(type);

            var notification = new Notification
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                NotificationType = type,
                EntityId = entityId,
                Message = message,
            };
            await _unitOfWork.NotificationRepository.AddNotificationAsync(notification);
            await _unitOfWork.ApplyChangesAsync();
            return notification;
        }

        public Task<PageResult<ClientNotificationDto>> GetNotificationsAsync(Guid recieverId, int pageNumber = 1)
        {
            throw new NotImplementedException();
        }

        public async Task MarkAsRead(Guid notificationId)
        {
            var notification = await _unitOfWork.NotificationRepository.GetNotificationAsync(notificationId);
            if (notification == null)
                throw new NullReferenceException("Notification not found");
            _unitOfWork.NotificationRepository.MarkAsRead(notification);
        }
    }
}

using Friendshub.Application.DTO;
using Friendshub.Application.Extensions;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Interfaces.SignalR;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Implementations
{
    internal class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationHub _hub;
        public NotificationService(IUnitOfWork unitOfWork, INotificationHub hub)
        {
            _unitOfWork = unitOfWork;
            _hub = hub;
            
        }
        public async Task CreateNotification(Guid senderId, Guid receiverId, NotificationType type, Guid? entityId = null)
        {
            //if (senderId == receiverId)
            //    return;
            //var sender = await _unitOfWork.UserRepository.GetUserByIdAsNoTracking(senderId);
            //if (sender == null)
            //    return;
            //var reciever = await  _unitOfWork.UserRepository.GetUserByIdAsNoTracking(senderId);
            //if (reciever == null)
            //    return;

            //var message = sender.Username.BuildNotificationMessage(type);

            //var notification = new Notification
            //{
            //    SenderId = senderId,
            //    ReceiverId = receiverId,
            //    NotificationType = type,
            //    EntityId = entityId,
            //    Message = message,
            //};
            //await _unitOfWork.NotificationRepository.AddNotificationAsync(notification);
            await _hub.SendNotificationAsync(receiverId, new
            {
                SenderId = senderId,
                EntityId = entityId,
                Type = type,
                CreatedAt = DateTime.UtcNow,

            });
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

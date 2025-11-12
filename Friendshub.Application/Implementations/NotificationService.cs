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
        public async Task CreateNotification(Guid senderId, Guid receiverId, NotificationType type, Guid? entityId = null)
        {
            if (senderId == receiverId)
                return;
            var sender = await _unitOfWork.UserRepository.GetUserByIdAsNoTracking(senderId);
            if (sender == null)
                return;
            var reciever = await  _unitOfWork.UserRepository.GetUserByIdAsNoTracking(senderId);
            if (reciever == null)
                return;

            var message = sender.Username.BuildNotificationMessage(type);

            var notification = new Notification
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                NotificationType = type,
                EntityId = entityId,
                Message = message,
            };
            await 
        }

        public Task<PageResult<ClientNotificationDto>> GetNotificationsAsync(Guid recieverId, int pageNumber = 1)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsRead(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

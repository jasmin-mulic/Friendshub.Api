using Friendshub.Api.Hubs;
using Friendshub.Application.DTO;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace Friendshub.Api.Services
{
    public class NotificationHubService : INotificationHubService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHubService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(Guid receiverId, object data)
        {
            await _hubContext.Clients.User(receiverId.ToString())
                .SendAsync("ReceiveNotification", data);
        }
    }
}

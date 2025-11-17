using Friendshub.Api.Hubs;
using Friendshub.Application.Interfaces.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace Friendshub.Api.Services
{
    public class NotificationHubService : INotificationHub
    {
        private readonly IHubContext<NotificationsHub> _hub;

        public NotificationHubService(IHubContext<NotificationsHub> hub)
        {
            _hub = hub;
        }
        public async Task SendNotificationAsync(Guid receiverId, object payload)
        {
            await _hub.Clients.User(receiverId.ToString())
                .SendAsync("ReceiveNotification", payload);
        }
    }
}

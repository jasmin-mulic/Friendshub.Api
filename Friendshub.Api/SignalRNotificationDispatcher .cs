using Friendshub.Api.Hubs;
using Friendshub.Application.Implementations;
using Microsoft.AspNetCore.SignalR;

namespace Friendshub.Api
{
    public class SignalRNotificationDispatcher : INotificationDispatcher
    {
        private readonly IHubContext<NotificationHub> _hub;
        public SignalRNotificationDispatcher(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public Task DispatchAsync(Guid receiverId, object notification)
        {
             return _hub.Clients
            .User(receiverId.ToString())
            .SendAsync("ReceiveNotification", notification);
        }
    }
}

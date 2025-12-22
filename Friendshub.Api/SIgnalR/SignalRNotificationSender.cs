using Friendshub.Api.SIgnalR.Hubs;
using Friendshub.Application.Interfaces;
using Friendshub.Domain.Models;
using Microsoft.AspNetCore.SignalR;

namespace Friendshub.Api.SIgnalR
{
    public class SignalRNotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public SignalRNotificationSender(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendAsync(Guid receiverId, Notification notification)
        {
            await _hubContext.Clients.User(receiverId.ToString())
                  .SendAsync("ReceiveNotification", notification);
        }
    }
}

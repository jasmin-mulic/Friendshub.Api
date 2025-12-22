using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
namespace Friendshub.Api.SIgnalR.Hubs
{
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                Console.WriteLine($"User connected: {userId} | ConnectionId: {Context.ConnectionId}");
            }
            return base.OnConnectedAsync();
        }
    }

}

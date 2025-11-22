using Friendshub.Api.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace Friendshub.Api.SignalR
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // userId dolazi iz JWT tokena jer ga frontend šalje preko accessTokenFactory
            var userId = Context.User.GetUserId().ToString();
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            await base.OnConnectedAsync();
        }
    }
}

using Microsoft.AspNetCore.SignalR;

namespace Friendshub.Api.SignalR
{
    public class PostLikeHub : Hub
    {
        public async Task LikePost(Guid SenderId, Guid RecieverId, Guid PostId)
        {
            await Clients.All.SendAsync("LikePost", PostId);
        }
    }
}

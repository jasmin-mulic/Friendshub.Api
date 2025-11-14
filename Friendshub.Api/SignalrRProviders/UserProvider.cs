using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Friendshub.Api.SignalrRProviders
{
    public class UserProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext context)
        {
            return context.User.FindFirst("id").Value!;
        }
    }
}

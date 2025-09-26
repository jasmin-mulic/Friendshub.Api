using System.Security.Claims;

namespace Friendshub.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId (this ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId != null ? Guid.Parse(userId) : Guid.Empty;
        }
    }
}

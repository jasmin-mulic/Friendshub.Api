using Friendshub.Domain.Models;

namespace Friendshub.Application.Extensions
{
    public static class StringExtensions
    {
        private const string BackendBaseUrl = "https://localhost:7291/";

        public static string ToFullImagePath(this string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            relativePath = relativePath.TrimStart('/');

            return $"{BackendBaseUrl}{relativePath}";
        }
        public static string BuildNotificationMessage(this string senderName, NotificationType type)
        {
            return type switch
            {
                NotificationType.Follow => $"{senderName} followed you.",
                NotificationType.Like => $"{senderName} liked your post.",
                NotificationType.Comment => $"{senderName} commented on your post.",
                NotificationType.Request => $"{senderName} sent you follow request.",
                _ => $"{senderName} sent you a notification."
            };
        }
    }
}

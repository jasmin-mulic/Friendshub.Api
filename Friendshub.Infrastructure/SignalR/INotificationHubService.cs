namespace Friendshub.Infrastructure.SignalR
{
    public interface INotificationHubService
    {
        Task SendNotificationAsync(Guid receiverId, object data);
    }
}

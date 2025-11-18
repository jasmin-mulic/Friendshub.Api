namespace Friendshub.Application.Interfaces.SignalR
{
    public interface INotificationHub
    {
        Task SendNotificationAsync(Guid receiverId, object payload);

    }
}

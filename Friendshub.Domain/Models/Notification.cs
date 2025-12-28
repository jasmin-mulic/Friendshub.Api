namespace Friendshub.Domain.Models
{
    public class Notification : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public NotificationType NotificationType { get; set; }
        public Guid? EntityId { get; set; }
        public bool IsOpened { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Message { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
    public enum NotificationType
    {
        Follow = 1,
        Like = 2,
        Request = 3,
        Comment = 4,
    }
}

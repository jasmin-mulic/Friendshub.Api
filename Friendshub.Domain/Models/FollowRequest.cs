namespace Friendshub.Domain.Models
{
    public class FollowRequest
    {
        public Guid SenderId { get; set; }
        public User Sender { get; set; }
        public Guid RecieverId { get; set; }
        public User Reciever {  set; get; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}

namespace Friendshub.Domain.Models
{
    public class FollowRequest
    {
        public Guid SenderId { get; set; }
        public virtual User Sender { get; set; }
        public Guid RecieverId { get; set; }
        public virtual User Reciever {  set; get; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}

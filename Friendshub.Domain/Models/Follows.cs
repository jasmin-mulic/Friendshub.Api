namespace Friendshub.Domain.Models
{
    public class Follows
    {
        public Guid FolloweeId { get; set; }
        public Guid FollowerId { get; set; }
        public User Follower { get; set; }
        public User Followee { get; set; }

    }
}

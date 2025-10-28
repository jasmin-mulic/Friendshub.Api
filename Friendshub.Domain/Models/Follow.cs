namespace Friendshub.Domain.Models
{
    public class Follow : BaseEntity
    {
        public Guid FolloweeId { get; set; }
        public Guid FollowerId { get; set; }
        public virtual User Follower { get; set; }
        public virtual User Followee { get; set; }

    }
}

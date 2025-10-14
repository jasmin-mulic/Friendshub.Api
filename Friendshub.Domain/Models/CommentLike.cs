namespace Friendshub.Domain.Models
{
    public class CommentLike
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public Guid CommentId { get; set; }
        public virtual Comment Comment { get; set; }

        public DateTime LikedAt { get; set; } = DateTime.UtcNow;

    }
}

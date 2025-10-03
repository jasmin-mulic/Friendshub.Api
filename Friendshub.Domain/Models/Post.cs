namespace Friendshub.Domain.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        public int LikeCounter { get; set; }
        public User User { get; set; }
        public virtual ICollection<PostImage> PostsImages { get; set; } = new List<PostImage>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}

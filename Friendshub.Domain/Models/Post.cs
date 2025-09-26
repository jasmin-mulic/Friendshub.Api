namespace Friendshub.Domain.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public DateTime PostedAt { get; set; }
        public int LikeCounter { get; set; }
        public string User { get; set; }
        public virtual ICollection<PostImage> PostsImages { get; set; } = new List<PostImage>();
    }
}

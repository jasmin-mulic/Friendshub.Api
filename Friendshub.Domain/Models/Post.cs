using System.Security.Claims;

namespace Friendshub.Domain.Models
{
    public class Post : BaseEntity
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        public virtual User User { get; set; }
        public virtual ICollection<PostImage> PostsImages { get; set; } = new List<PostImage>();
        public virtual ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
        public virtual ICollection<Comment> Comments { get; set; }
    }

}

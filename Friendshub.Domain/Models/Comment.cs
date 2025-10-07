using System.Diagnostics.CodeAnalysis;

namespace Friendshub.Domain.Models
{
    public class Comment
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string CommentImageUrl { get; set; } = string.Empty;
        public User User { get; set; }
        public Post Post { get; set; }
        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;





    }
}

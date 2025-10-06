using System.Diagnostics.CodeAnalysis;

namespace Friendshub.Domain.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        [AllowNull]
        public string Content { get; set; } = string.Empty;
        public Guid PostId { get; set; }
        [AllowNull]
        public string CommentImageUrl { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;





    }
}

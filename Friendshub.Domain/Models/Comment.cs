using System.Diagnostics.CodeAnalysis;

namespace Friendshub.Domain.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string CommentImageUrl { get; set; } = string.Empty;
        public virtual  Post Post { get; set; }
        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;
        public virtual  List<CommentLike> CommentLikes { get; set; } = new();
    }
}

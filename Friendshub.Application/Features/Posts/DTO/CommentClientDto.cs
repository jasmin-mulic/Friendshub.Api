using Friendshub.Application.Features.Users.DTO;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Features.Posts.DTO
{
    public class CommentClientDto
    {
        public Guid UserId { get; set; }
        public Guid CommentId { get; set; }
        public string Content { get; set; }
        public string UserProfileImageUrl { get; set; }
        public string Username { get; set; }
        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;
        public string CommentImageUrl { get; set; }
        public List<UserBasicInfo> CommentLikes { get; set; } = new();

    }
}

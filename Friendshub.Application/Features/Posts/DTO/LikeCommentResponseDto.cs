using Friendshub.Application.Features.Users.DTO;


namespace Friendshub.Application.Features.Posts.DTO
{
    public class LikeCommentResponseDto
    {
        public Guid CommentId { get; set; }
        public UserBasicInfo User { get; set; } = new UserBasicInfo();
        public bool IsLiked { get; set; }
    }
}

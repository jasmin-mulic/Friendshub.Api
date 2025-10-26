using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Domain.Models;


namespace Friendshub.Application.DTO.PostDto
{
    public class LikeCommentResponseDto
    {
        public Guid CommentId { get; set; }
        public string Message { get; set; }
        public UserBasicInfo User { get; set; } = new UserBasicInfo();
    }
}

using Friendshub.Application.DTO.PostDto;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Services
{
    public interface ILIkeService
    {
        Task<LikeCommentResponseDto> LikePostComment(Guid commentId, Guid userId);
        Task<List<UserBasicInfo>> GetPostLikes(Guid PostId);
        Task<string> LikePost(Guid userId, Guid postId);

    }
}

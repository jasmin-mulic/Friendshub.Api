using Friendshub.Application.Features.Posts.DTO;
using Friendshub.Application.Features.Users.DTO;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Services
{
    public interface ILIkeService
    {
        Task<LikeCommentResponseDto> LikePostComment(Guid commentId, Guid userId);
        Task<List<UserBasicInfo>> GetPostLikes(Guid PostId);
        Task<bool> LikePost(Guid userId, Guid postId);
        Task<List<UserBasicInfo>> GetUserLikesAsync();

    }
}

using Friendshub.Application.DTO.PostDto;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Services
{
    public interface ILIkeService
    {
        Task<LikeCommentResponseDto> LikePostComment(Guid commentId, Guid userId);
        Task<List<PostLike>> GetPostLikes(Guid PostId);
        Task<string> LikePost(Guid userId, Guid postId);

    }
}

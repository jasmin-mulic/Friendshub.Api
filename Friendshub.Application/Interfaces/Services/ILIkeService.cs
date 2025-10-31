using Friendshub.Application.DTO.PostDto;

namespace Friendshub.Application.Interfaces.Services
{
    public interface ILIkeService
    {
        Task<LikeCommentResponseDto> LikePostComment(Guid commentId, Guid userId);
    }
}

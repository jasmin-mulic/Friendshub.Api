using Friendshub.Application.Features.Users.DTO;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Repositories
{
    public interface IPostLikeRepository
    {
        void RemoveLike(PostLike like);
        Task AddLike(PostLike like);
        Task<List<PostLike>> GetPostLikes(Guid postId);
        Task<PostLike> GetPostLikeForUser(Guid postId, Guid userId);
        Task<List<UserBasicInfo>> GetUserLikesAsync();
    }
}

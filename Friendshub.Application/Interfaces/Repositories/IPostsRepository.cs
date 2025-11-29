using Friendshub.Domain.Models;
namespace Friendshub.Application.Interfaces.Repositories
{
    public interface IPostRepository
    {
        Task AddPostAsync(Post post);
        Task<List<Post>> GetFeedPostsPaged(Guid userId,List<Guid> follows, int page);
        void DeletePost(Post post);
        Task<Post> GetPostByIdAsync(Guid postId);
        Task<List<Post>> GetUserPostsByIdPaged(Guid userId, int pageNumber, int pageSize);
        Task<int> UserPostTotalCount(Guid userId);
        Task<int> FeedPostsTotalCount(Guid userId, List<Guid> followingUsersIds);

    }
}

using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
using Friendshub.Domain.Models;
namespace Friendshub.Application.Interfaces.Repositories
{
    public interface IPostRepository
    {
        Task AddPostAsync(Post post);
        Task<List<Post>> GetFeedPostsPaged(Guid userId,List<Guid> follows, int page);
        void DeletePost(Post post);
        Task<Post> GetPostByIdAsync(Guid postId);
        Task<string> LikePost(Guid UserId, Guid PostId);
        Task<PageResult<PostClientDto>> GetUserPostsByIdPaged(Guid userId, int pageNumber, int pageSize);
        Task<List<PostLike>> GetPostLikes(Guid PostId);
        Task<CommentClientDto> AddCommentToPost(Guid userId, Post post, AddCommentDto comment);
        Task<int> GetUserPostCount(Guid userId);
        Task<int> FeedPostsTotalCount(Guid userId, List<Guid> followingUsersIds);
        Task<int> LoggedUserPostCount(Guid userId);

    }
}

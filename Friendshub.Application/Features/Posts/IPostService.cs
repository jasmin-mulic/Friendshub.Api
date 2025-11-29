using Friendshub.Application.DTO;
using Friendshub.Application.Features.Posts.DTO;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Features.Posts
{
    public interface IPostService
    {
        Task<PostClientDto> AddPost(AddPostDto request, Guid userId);
        Task<PageResult<PostClientDto>> GetLoggedUserPosts(Guid userId, int page);
        Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId, int page);
        Task<bool> DeletePost(Guid postId);
        Task<Post> GetPostByIdAsync(Guid postId);
    }
}

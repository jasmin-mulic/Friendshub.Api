using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<PostClientDto> AddPost(AddPostDto request, User user);
        Task<PageResult<PostClientDto>> GetMyPosts(Guid userId, int page);
        Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId, int page);
        Task<bool> DeletePost(Guid postId);
        Task<Post> GetPostById(Guid postId);
        Task<string> LikePost(Guid UserId, Guid PostId);
    }
}

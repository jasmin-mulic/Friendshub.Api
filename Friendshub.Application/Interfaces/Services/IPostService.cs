using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Services
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

using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Services
{
    public interface IPostService
    {
        Task<PostClientDto> AddPost(AddPostDto request, Guid userId);
        Task<PageResult<PostClientDto>> GetMyPosts(Guid userId, int page);
        Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId, int page);
        Task<bool> DeletePost(Guid postId);
        Task<Post> GetPostByIdAsync(Guid postId);
        Task<string> LikePost(Guid UserId, Guid PostId);
        Task<CommentClientDto> CommentPost(Guid userId, Post post, AddCommentDto comment);
        Task<Comment> GetCommentByIdAsync(Guid postId);
    }
}

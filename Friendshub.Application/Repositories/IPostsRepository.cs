using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
using Friendshub.Domain.Models;
namespace Friendshub.Application.Repositories
{
    public interface IPostRepository
    {
        Task<PostClientDto> AddPost(AddPostDto request, User user);
        Task<PageResult<PostClientDto>>GetMyPosts(Guid userId, int page);
        Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId, int page);
        Task<bool> DeletePost(Guid postId);
        Task<Post> GetPostById(Guid postId);
        Task<string> LikePost(Guid UserId, Guid PostId);
        Task<CommentClientDto> CommentPost (Guid userId, Post post, AddCommentDto comment);
        Task<LikeCommentResponseDto> LikeComment(Guid commentId, Guid userId);
        Task<Comment> GetCommentByIdAsync(Guid commentId);
        Task<bool> DeleteComment(Guid commentId, Guid userId);
    }
}

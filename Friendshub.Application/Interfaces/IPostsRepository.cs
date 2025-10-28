using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
using Friendshub.Domain.Models;
namespace Friendshub.Application.Repositories
{
    public interface IPostRepository
    {
        Task AddPostAsync(Post post);
        Task<List<Post>>GetPostsByUserId(Guid userId);
        Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId, int page);
        void DeletePost(Post post);
        Task<Post> GetPostByIdAsync(Guid postId);
        Task<string> LikePost(Guid UserId, Guid PostId);
        Task<CommentClientDto> CommentPost (Guid userId, Post post, AddCommentDto comment);
        Task<LikeCommentResponseDto> LikePostComment(Guid commentId, Guid userId);
        Task<Comment> GetCommentById(Guid commentId);
        Task<bool> DeleteComment(Guid commentId, Guid userId);
    }
}

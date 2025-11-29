using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Repositories
{
    public interface ICommentLikeRepository
    {
        Task<CommentLike> GetUserLikeAsync(Guid userId, Guid commentId);
        void RemoveCommentLike(CommentLike comment);
        Task AddCommentLike(CommentLike commentLike);
    }
}

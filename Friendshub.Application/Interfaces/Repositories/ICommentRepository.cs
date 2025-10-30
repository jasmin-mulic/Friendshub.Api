using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Repositories
{
    public interface ICommentRepository
    {
        Task<Comment> GetByIdAsync(Guid id);
        Task AddAsync(Comment comment);
        void Delete(Comment comment);
    }
}

using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Friendshub.Infrastructure.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly FriendshubDbContext _context;
        public CommentRepository(FriendshubDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }

        public void Delete(Comment comment)
        {
            _context.Comments.Remove(comment);
        }

        public async Task<Comment> GetByIdAsync(Guid id)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Comment> GetPostCommentById(Guid commentId)
        {
            return await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == commentId);
        }
    }
}

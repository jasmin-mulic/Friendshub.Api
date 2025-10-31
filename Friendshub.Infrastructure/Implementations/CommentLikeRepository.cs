using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Infrastructure.Implementations
{
    public class CommentLikeRepository : ICommentLikeRepository
    {
        private readonly FriendshubDbContext _context;
        public CommentLikeRepository(FriendshubDbContext context)
        {
                _context = context;
        }
        public async Task<CommentLike> GetUserLikeAsync(Guid userId, Guid commentId)
        {
            return await _context.CommentsLikes.FirstOrDefaultAsync(x => x.UserId == userId && x.CommentId == commentId);
        }
    }
}

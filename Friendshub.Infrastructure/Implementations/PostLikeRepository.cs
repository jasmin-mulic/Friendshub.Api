using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Friendshub.Infrastructure.Implementations
{
    public class PostLikeRepository : IPostLikeRepository
    {
        private readonly FriendshubDbContext _context;
        public PostLikeRepository(FriendshubDbContext context)
        {
            _context = context;
        }
        public async Task AddLike(PostLike like)
        {
            await _context.PostLikes.AddAsync(like);
        }

        public async Task<List<PostLike>> GetPostLikes(Guid postId)
        {
            return await _context.PostLikes.AsNoTracking().Where(x => x.PostId == postId).ToListAsync();
        }

        public async Task<PostLike> GetPostLikeForUser(Guid postId, Guid userId)
        {
            return await _context.PostLikes.Include(x => x.Post).FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
        }

        public void RemoveLike(PostLike postLike) 
        {
            _context.PostLikes.Remove(postLike);
        }
    }
}

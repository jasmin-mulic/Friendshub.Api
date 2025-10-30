using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Friendshub.Infrastructure.Implementations
{
    public class FollowRepository : IFollowRepository
    {
        private readonly FriendshubDbContext _context;
        public FollowRepository(FriendshubDbContext context)
        {
            _context = context;
        }
        public async Task<List<Guid>> GetUserFollowingList(Guid userId)
        {
            return await _context.Follows.AsNoTracking().Where(x => x.FollowerId == userId).Select(x => x.FollowerId).ToListAsync();
        }

        public void RemoveFollows(List<Follow> follows)
        {
            _context.Follows.RemoveRange(follows);
        }
    }
}

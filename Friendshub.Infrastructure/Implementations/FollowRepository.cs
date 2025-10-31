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

        public async Task<List<Guid>> GetFollowingUsersIds(Guid followerId)
        {
            return await _context.Follows.AsNoTracking().Where(x => x.FollowerId == followerId).Select(x => x.FolloweeId).ToListAsync();
        }
        

        public async Task<List<Follow>> GetUserFollowingList(Guid userId)
        {
            return await _context.Follows.AsNoTracking().Where(x => x.FollowerId == userId).ToListAsync();
        }

        public void RemoveFollows(List<Follow> follows)
        {
            _context.Follows.RemoveRange(follows);
        }

        public void RemoveFollows(List<Guid> follows)
        {
            throw new NotImplementedException();
        }
    }
}

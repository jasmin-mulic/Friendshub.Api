using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace Friendshub.Infrastructure.Implementations
{
    public class FollowRepository : IFollowRepository
    {
        private readonly FriendshubDbContext _context;
        public FollowRepository(FriendshubDbContext context)
        {
            _context = context;
        }

        public async Task<Follow> GetByIdAsync(Guid followerId, Guid foloweeId)
        {
            return await _context.Follows.FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FolloweeId == foloweeId);
        }

        public async Task<List<Guid>> GetFollowingUsersIds(Guid followerId)
        {
            return await _context.Follows.AsNoTracking().Where(x => x.FollowerId == followerId).Select(x => x.FolloweeId).ToListAsync();
        }
        public async Task<List<Follow>> GetUserFollowingList(Guid userId)
        {
            return await _context.Follows.AsNoTracking().Where(x => x.FolloweeId == userId).ToListAsync();
        }

        public void DeleteFollow(Follow follow)
        {
            _context.Follows.RemoveRange(follow);
        }

        public void RemoveFollows(List<Follow> follows)
        {
            _context.Follows.RemoveRange(follows);
        }

        public async Task AddFollowAsync(Follow follow)
        {
            await _context.Follows.AddAsync(follow);
        }

        public async Task<List<User>> GetFollowRecommendationsAsync(Guid userId, int skip, int take)
        {
            var query = await _context.Users
                .AsNoTracking()
                .Where(u => !_context.Follows
                    .Where(f => f.FollowerId == userId)
                    .Select(f => f.FolloweeId)
                    .Contains(u.Id)
                    && u.Id != userId).ToListAsync();

            return query;
        }
        public async Task<int> GetFollowRecommendationsCountAsync(Guid userId)
        {
           
            var count =  await _context.Users
                .AsNoTracking()
                .Where(u => !_context.Follows
                    .Where(f => f.FollowerId == userId)
                    .Select(f => f.FolloweeId)
                    .Contains(u.Id)
                    && u.Id != userId)
                .CountAsync();

            return count;
        }

        public async Task<List<User>> GetUserFollowings(Guid userId)
        {
            return await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.Followee)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<User>> GetUserFollowers(Guid userId)
        {
            return await _context.Follows
                .Where(f => f.FolloweeId == userId)
                .Select(f => f.Follower)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetUserFollowersCount(Guid userId)
        {
            return await _context.Follows
                .Where(f => f.FolloweeId == userId)
                .AsNoTracking()
                .CountAsync();
        }

        public async Task<int> GetFollowingCount(Guid userId)
        {
            return await _context.Follows
                .Where(f => f.FollowerId == userId)
                .AsNoTracking()
                .CountAsync();
        }
    }
    }

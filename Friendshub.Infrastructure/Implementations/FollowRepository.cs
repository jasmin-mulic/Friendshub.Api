using Friendshub.Application.DTO;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Extensions;
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
        

        public async Task<List<UserBasicInfo>> GetUserFollowingsList(Guid userId)
        {
            return await _context.Follows
                            .AsNoTracking().Where(x => x.FollowerId == userId)
                            .Select(u => new UserBasicInfo
                            {
                                ProfileImageUrl = u.Follower.ProfileImageUrl == null ? null : u.Follower.ProfileImageUrl.ToFullImageUrl(),
                                Username = u.Follower.Username,
                                UserId = u.FollowerId,
                            }).ToListAsync();
        }
        public async Task<List<UserBasicInfo>> GetUserFollowersList(Guid userId)
        {
            return await _context.Follows
                            .AsNoTracking().Where(x => x.FolloweeId == userId)
                            .Select(u => new UserBasicInfo
                            {
                                ProfileImageUrl = u.Followee.ProfileImageUrl == null ? null : u.Follower.ProfileImageUrl.ToFullImageUrl(),
                                Username = u.Followee.Username,
                                UserId = u.FolloweeId,
                            }).ToListAsync();
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

        public async Task<List<User>> GetFollowRecommendations(Guid userId, int skip, int take)
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
    }
}

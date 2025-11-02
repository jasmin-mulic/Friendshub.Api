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

        public async Task<PageResult<UserBasicInfo>> GetFollowRecommendations(Guid userId, int pageNumber, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize != 10) pageSize = 10;

            var followingUsersIds = await GetFollowingUsersIds(userId);

            var query =  _context.Users
                .AsNoTracking()
                .Where(u => !_context.Follows
                    .Where(f => f.FollowerId == userId)
                    .Select(f => f.FolloweeId)
                    .Contains(u.Id)
                    && u.Id != userId);

            var totalCount = await query.CountAsync();

            var recommendations = query.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .Select(x => new UserBasicInfo
                                 {
                                     UserId = x.Id,
                                     Username = x.Username,
                                     ProfileImageUrl = x.ProfileImageUrl == null ? null : x.ProfileImageUrl.ToFullImageUrl()
                                 })
                                 .ToList();

            var pageResult = new PageResult<UserBasicInfo>
            {
                Items = recommendations,
                PageNumber = pageNumber,
                TotalCount = totalCount,
                PageSize = pageSize
            };
            return pageResult;
        }
    }
}

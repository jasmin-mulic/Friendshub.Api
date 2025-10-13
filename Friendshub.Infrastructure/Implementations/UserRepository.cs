using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Extensions;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Friendshub.Infrastructure.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly FriendshubDbContext _context;
        private readonly IWebHostEnvironment _env;
        public UserRepository(FriendshubDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<string> ChangeProfilePicture(IFormFile file)
        {
            if(file == null || file.Length == 0)
                throw new ArgumentNullException("Invalid file.");

            var uploadPath = Path.Combine(_env.WebRootPath, "uploads/profileImages");
            if(!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/profileImages/{fileName}";
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            var follows = await _context.Follows.Where(x => x.FollowerId == id).ToListAsync();
             _context.Follows.RemoveRange(follows);
            _context.Users.Remove(user);
        }

        public async Task<string> FollowUser(Guid followerId, Guid followeeId)
        {
            string responseMessage = string.Empty;

            var follow = await _context.Follows.FirstOrDefaultAsync(x => x.FollowerId == followerId &&
                                                                     x.FolloweeId == followeeId);

            if (follow != null)
            {
                _context.Follows.Remove(follow);
                responseMessage = "unfollowed";

            }
            else
            {
                var followee = await _context.Users.FirstOrDefaultAsync(x => x.Id == followeeId);

                if(followee.PrivateAccount)
                {
                    var newFollowRequest = new FollowRequest
                    {
                        SenderId = followerId,
                        RecieverId = followeeId
                    };
                    responseMessage = "Follow request sent";
                    return responseMessage;
                };

                var newFollow = new Follows
                {
                    FollowerId = followerId,
                    FolloweeId = followeeId
                };
                _context.Follows.Add(newFollow);
                responseMessage = "followed";
            }
            return responseMessage;
        }

        public async Task<User> GetById(Guid id)
        {
           var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<List<UserBasicInfo>> GetFollowers(Guid userId)
        {
            var followers = await _context.Follows.Include(f => f.Follower).Include(f => f.Followee).Where(x => x.FolloweeId == userId).Select(f => new UserBasicInfo
            {
                ProfileImageUrl = f.Follower.ProfileImgUrl.ToFullImageUrl(),
                UserId = f.FollowerId,
                Username = f.Follower.DisplayUsername
            }).AsNoTracking().ToListAsync();
            return followers;
        }
        public async Task<List<UserBasicInfo>> GetFollowings(Guid userId)
        {
            var followingList = await _context.Follows.Include(x => x.Follower).Where(x =>x.FollowerId == userId).Select(f => new UserBasicInfo
            {
                ProfileImageUrl = f.Followee.ProfileImgUrl.ToFullImageUrl(),
                UserId = f.Followee.Id,
                Username= f.Followee.DisplayUsername
            }).ToListAsync();
            return followingList;

        }

        public async Task<List<FollowRecommendationDto>> GetFollowRecommendationList(Guid userId)
        {
            // Dobavi sve FolloweeId koje korisnik već prati
            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FolloweeId)
                .ToListAsync();

            // Ako ne prati nikoga, random fallback 20 korisnika koje ne uključuju njega
            if (!followingIds.Any())
            {
                return await _context.Users
                    .Where(u => u.Id != userId) // ne predlaži samog sebe
                    .OrderBy(u => Guid.NewGuid())
                    .Take(20)
                    .Select(u => new FollowRecommendationDto
                    {
                        Id = u.Id,
                        Username = u.DisplayUsername,
                        ProfileImageUrl = u.ProfileImgUrl.ToFullImageUrl(),
                    })
                    .ToListAsync();
            }

            // Inače preporuke: korisnici koje ne prati
            var recommendations = await _context.Users
                .Where(u => u.Id != userId && !followingIds.Contains(u.Id))
                .OrderBy(u => Guid.NewGuid())
                .Take(20)
                .Select(u => new FollowRecommendationDto
                {
                    Id = u.Id,
                    Username = u.DisplayUsername,
                    ProfileImageUrl = u.ProfileImgUrl.ToFullImageUrl(),
                })
                .ToListAsync();

            return recommendations;
        }

        public async Task<ProfileDataDto> GetProfileData(User request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (user == null)
                return null;
            var followersCount = await _context.Follows.CountAsync(x => x.FolloweeId == request.Id);
            var followingCount = await _context.Follows.CountAsync(x => x.FollowerId == request.Id);
            var postCount = await _context.Posts.CountAsync(x => x.UserId == request.Id);

            var userProfileData = new ProfileDataDto
            {
                DisplayUsername = user.Username,
                ProfileImgUrl = string.IsNullOrWhiteSpace(user.ProfileImgUrl) ? null : user.ProfileImgUrl.ToFullImageUrl(),
                FollowersCount = followersCount,
                FollowingCount = followingCount,
                PostCount = postCount
            };
            return userProfileData;
        }

        public void RemoveFollower(Guid followeeId, Guid followerId)
        {
            var follow = _context.Follows.FirstOrDefault(f => f.FolloweeId.Equals(followeeId)
                                                        && f.FollowerId.Equals(followerId));
            if (follow == null)
                throw new ApplicationException("User is not following you.");
            _context.Follows.Remove(follow);
        }
    }
}

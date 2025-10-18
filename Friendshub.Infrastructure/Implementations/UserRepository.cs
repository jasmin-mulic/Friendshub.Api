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

        #region Profile Picture
        public async Task<string> ChangeProfilePicture(Guid userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentNullException("Invalid file.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new ApplicationException("User not found.");

            var uploadDir = Path.Combine(_env.WebRootPath, "uploads/profileImages");
            Directory.CreateDirectory(uploadDir);

            if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
            {
                var oldPath = Path.Combine(_env.WebRootPath, user.ProfileImageUrl);
                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var relativePath = Path.Combine("uploads/profileImages", fileName).Replace("\\", "/");
            var fullPath = Path.Combine(_env.WebRootPath, relativePath);

            using (var stream = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(stream);

            user.ProfileImageUrl = relativePath;
            _context.Users.Update(user);

            return relativePath.ToFullImageUrl();
        }
        #endregion

        #region User Management
        public async Task DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new ApplicationException("User not found.");

            var follows = await _context.Follows.Where(x => x.FollowerId == id).ToListAsync();
            _context.Follows.RemoveRange(follows);
            _context.Users.Remove(user);
        }
        #endregion

        #region Follow System
        public async Task<string> FollowUser(Guid followerId, Guid followeeId)
        {
            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(x => x.FollowerId == followerId && x.FolloweeId == followeeId);

            if (existingFollow != null)
            {
                _context.Follows.Remove(existingFollow);
                return "unfollowed";
            }

            var pendingRequest = await _context.FollowRequests
                .FirstOrDefaultAsync(x => x.SenderId == followerId && x.RecieverId == followeeId);

            if (pendingRequest != null)
            {
                _context.FollowRequests.Remove(pendingRequest);
                return "Follow request canceled.";
            }

            var followee = await _context.Users.FirstOrDefaultAsync(x => x.Id == followeeId);
            if (followee == null)
                throw new ApplicationException("Followee not found.");

            if (!followee.PrivateAccount)
            {
                await _context.Follows.AddAsync(new Follows
                {
                    FollowerId = followerId,
                    FolloweeId = followeeId
                });
                return "followed";
            }

                await _context.FollowRequests.AddAsync(new FollowRequest
                {
                    SenderId = followerId,
                    RecieverId = followeeId
                });
            return "Follow request sent";
        }
        #endregion

        #region Getters
        public async Task<User> GetUserById(Guid id)
            => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<List<UserBasicInfo>> GetFollowers(Guid userId)
            => await _context.Follows
                .Include(f => f.Follower)
                .Where(x => x.FolloweeId == userId)
                .Select(f => new UserBasicInfo
                {
                    ProfileImageUrl = f.Follower.ProfileImageUrl.ToFullImageUrl(),
                    UserId = f.FollowerId,
                    Username = f.Follower.Username
                })
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<UserBasicInfo>> GetFollowings(Guid userId)
            => await _context.Follows
                .Include(f => f.Followee)
                .Where(x => x.FollowerId == userId)
                .Select(f => new UserBasicInfo
                {
                    ProfileImageUrl = f.Followee.ProfileImageUrl.ToFullImageUrl(),
                    UserId = f.Followee.Id,
                    Username = f.Followee.Username
                })
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<FollowRecommendationDto>> GetFollowRecommendationList(Guid userId)
        {
            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FolloweeId)
                .ToListAsync();

            var query = _context.Users
                .Where(u => u.Id != userId && !followingIds.Contains(u.Id))
                .OrderBy(u => Guid.NewGuid())
                .Take(20)
                .Select(u => new FollowRecommendationDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    ProfileImageUrl = u.ProfileImageUrl.ToFullImageUrl(),
                });

            if (!followingIds.Any())
                query = _context.Users
                    .Where(u => u.Id != userId)
                    .OrderBy(u => Guid.NewGuid())
                    .Take(20)
                    .Select(u => new FollowRecommendationDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        ProfileImageUrl = u.ProfileImageUrl.ToFullImageUrl(),
                    });

            return await query.ToListAsync();
        }

        public async Task<ProfileDataDto> GetProfileData(User request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (user == null)
                return null;

            return new ProfileDataDto
            {
                Username = user.Username,
                ProfileImageUrl = string.IsNullOrWhiteSpace(user.ProfileImageUrl)
                    ? null
                    : user.ProfileImageUrl.ToFullImageUrl(),
                FollowersCount = await _context.Follows.CountAsync(x => x.FolloweeId == user.Id),
                FollowingCount = await _context.Follows.CountAsync(x => x.FollowerId == user.Id),
                EmailAddress = request.EmailAddress,
                PostCount = await _context.Posts.CountAsync(x => x.UserId == user.Id),
                PrivateAccount = user.PrivateAccount
            };
        }

        public void RemoveFollower(Guid followeeId, Guid followerId)
        {
            var follow = _context.Follows
                .FirstOrDefault(f => f.FolloweeId == followeeId && f.FollowerId == followerId);

            if (follow == null)
                throw new ApplicationException("User is not following you.");

            _context.Follows.Remove(follow);
        }
        #endregion

        #region Update User
        public async Task<UpdateUserValidationDto> UpdateUserInfo(Guid id, UpdateUserInfoDto updateUserInfo)
        {
            var user = await GetUserById(id);
            if (user == null)
                throw new ApplicationException("Account not found.");

            var errors = new UpdateUserValidationDto();

            // Validation
            var usernameTaken = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == updateUserInfo.Username && u.Id != id);
            if (usernameTaken != null)
                errors.Username = "Username is taken.";

            var emailTaken = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == updateUserInfo.EmailAddress && u.Id != id);
            if (emailTaken != null)
                errors.EmailAddress = "Email address is taken.";

            if (!string.IsNullOrEmpty(errors.Username) || !string.IsNullOrEmpty(errors.EmailAddress))
                return errors;

            // Update image
            if (updateUserInfo.ProfileImageUrl != null && updateUserInfo.ProfileImageUrl.Length > 0)
            {
                var uploadDir = Path.Combine(_env.WebRootPath, "uploads/profileImages");
                Directory.CreateDirectory(uploadDir);

                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, user.ProfileImageUrl);
                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(updateUserInfo.ProfileImageUrl.FileName)}";
                var relativePath = Path.Combine("uploads/profileImages", fileName).Replace("\\", "/");
                var fullPath = Path.Combine(_env.WebRootPath, relativePath);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                    await updateUserInfo.ProfileImageUrl.CopyToAsync(stream);

                user.ProfileImageUrl = relativePath;
            }

            // Update info
            user.Username = updateUserInfo.Username.ToLower();
            user.PrivateAccount = updateUserInfo.PrivateAccount;

            _context.Users.Update(user);
            return errors;
        }
        #endregion
    }
}

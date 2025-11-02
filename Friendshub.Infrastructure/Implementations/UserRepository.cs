using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Extensions;
using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
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
        public async Task<User> GetUserById(Guid id)
            => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<MyProfileData> GetMyProfileData(User request)
        {
            var user = await _context.Users.Include(x => x.Posts).ThenInclude(p => p.PostsImages).FirstOrDefaultAsync(x => x.Id == request.Id);
            if (user == null)
                return null;

            return new MyProfileData
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
        public async Task<UserProfileData> GetUserProfileData(string username)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
                return null;

            var userData = new UserProfileData();
            userData.Username = user.Username;
            userData.UserId = user.Id;
            userData.PrivateAccount = user.PrivateAccount;
            userData.ProfileImageUrl = user.ProfileImageUrl == null ? null : user.ProfileImageUrl.ToFullImageUrl();

            if (!user.PrivateAccount)
            {
                var postsDto = await _context.Posts.Include(p => p.PostsImages)
                            .Include(p => p.Comments)
                            .ThenInclude(c => c.CommentLikes)
                            .Where(p => p.UserId == user.Id)
                            .Select(x => new PostClientDto
                            {
                                PostId = x.Id,
                                PostedAt = x.PostedAt,
                                Username = x.User.Username,
                                Content = x.Content,
                                ProfileImgUrl = x.User.ProfileImageUrl,
                                UserId = x.UserId,
                                Likes = _context.PostLikes.Include("User").Where(like => like.PostId == x.Id).Select((l => new UserBasicInfo
                                {
                                    UserId = l.UserId,
                                    ProfileImageUrl = l.User.ProfileImageUrl == null ? null : l.User.ProfileImageUrl.ToFullImageUrl(),
                                    Username = l.User.Username,
                                })).ToList(),
                                Comments = _context.Comments.Where(x => x.PostId == x.Id)
                                .Select(c => new CommentClientDto
                                {
                                    CommentedAt = c.CommentedAt,
                                    UserId = c.UserId,
                                    CommentImageUrl = c.CommentImageUrl,
                                    Content = c.Content,
                                    Username = c.User.Username,
                                    CommentLikes = _context.CommentLikes.Where(x => x.CommentId == c.Id)
                                          .Select(x => new UserBasicInfo
                                          {
                                              UserId = x.UserId,
                                              ProfileImageUrl = x.User.ProfileImageUrl == null ? null : x.User.ProfileImageUrl.ToFullImageUrl(),
                                          }).ToList()
                                }).ToList(),
                            }).ToListAsync();
                userData.Posts = postsDto;
            }
            return userData;
        }
        public async Task<User> GetUserByEmailOrUsername(string emailOrUsername)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.EmailAddress == emailOrUsername || x.Username == emailOrUsername);
        }

        public async Task<bool> IsUsernameTaken(string username)
        {
            var isTaken = await _context.Users.AnyAsync(x => x.Username == username);
            return isTaken;
        }

        public async Task<bool> IsEmailAddressTaken(string emailAddress)
        {
            var isTaken = await _context.Users.AnyAsync(x => x.EmailAddress == emailAddress);
            return isTaken;
        }
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);    
        }

        public void DeleteUser(User user)
        {
            user.IsDeleted = true;
            user.IsActive = false;
        }

        public async Task<User> GetByIdAsNoTracking(Guid id)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id); 
        }

        public void UpdateUserInfo(User user)
        {
            _context.Update(user);
        }
    }
}

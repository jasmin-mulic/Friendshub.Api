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

        public async Task<User> GetUserProfileDataAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Posts)
                    .ThenInclude(p => p.PostsImages)
                .FirstOrDefaultAsync(u => u.Id == userId);
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

        public async Task<User> GetUserByUsernameAsNoTracking(string username)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username);    
        }
    }
}

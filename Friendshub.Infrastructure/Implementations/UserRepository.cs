using Friendshub.Application.DTO.User;
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
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            if(!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/uploads/{fileName}";
        }

        public void FollowUser(Guid folowerId, Guid foloweeId)
        {
            var follow = new Follows
            {
                FollowerId = folowerId,
                FolloweeId = foloweeId
            };
            _context.Follows.Add(follow);
        }

        public async Task<User> GetById(Guid id)
        {
           var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<List<FollowRecommendation>> GetFollowRecommendationList(Guid userId)
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
                    .Select(u => new FollowRecommendation
                    {
                        Id = u.Id,
                        Username = u.DisplayUsername,
                        ProfileImageUrl = u.ProfileImgUrl
                    })
                    .ToListAsync();
            }

            // Inače preporuke: korisnici koje ne prati
            var recommendations = await _context.Users
                .Where(u => u.Id != userId && !followingIds.Contains(u.Id))
                .OrderBy(u => Guid.NewGuid())
                .Take(20)
                .Select(u => new FollowRecommendation
                {
                    Id = u.Id,
                    Username = u.DisplayUsername,
                    ProfileImageUrl = u.ProfileImgUrl
                })
                .ToListAsync();

            return recommendations;
        }

        public async Task<ProfileDataDto> GetProfileData(User request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (user == null)
                return null;
            var userProfileData = new ProfileDataDto
            {
                DisplayUsername = user.Username,
                ProfileImgUrl = "https://localhost:44326/" + user.ProfileImgUrl,
            };
            return userProfileData;
        }
    }
}

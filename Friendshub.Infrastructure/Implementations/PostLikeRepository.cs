using Dapper;
using Friendshub.Application.Features.Users.DTO;
using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace Friendshub.Infrastructure.Implementations
{
    public class PostLikeRepository : IPostLikeRepository
    {
        private readonly FriendshubDbContext _context;
        private readonly IDbConnection dbConnection;
        public PostLikeRepository(FriendshubDbContext context)
        {
            _context = context;
        }
        public async Task AddLike(PostLike like)
        {
            await _context.PostLikes.AddAsync(like);
        }

        public async Task<List<PostLike>> GetPostLikes(Guid postId)
        {
            return await _context.PostLikes.AsNoTracking().Where(x => x.PostId == postId).ToListAsync();
        }

        public async Task<PostLike> GetPostLikeForUser(Guid postId, Guid userId)
        {
            return await _context.PostLikes.Include(x => x.Post).FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
        }

        public void RemoveLike(PostLike postLike) 
        {
            _context.PostLikes.Remove(postLike);
        }

        public async Task<List<UserBasicInfo>> GetUserLikesAsync()
        {
            // Register IDbConnection in program.cs
            // Give it connection from appsettings
            // Use IDbConnection to call Stored Procedures
            // Create stored procedures directly in SSMS
            using var connection = new SqlConnection("Server=WALTER\\SQLEXPRESS;Database=FriendshubDB;trusted_connection=true;trustservercertificate=true;");

            var result = await connection.QueryAsync<UserBasicInfo>(
                "GetUserLikes",
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
    }


}

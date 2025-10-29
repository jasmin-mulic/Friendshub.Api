using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Friendshub.Infrastructure.Implementations
{
    public class TokenRepository : ITokenRepository
    {
        private readonly FriendshubDbContext _context;
        private readonly IConfiguration _configuration;
        public TokenRepository(FriendshubDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public async Task AddRefreshTokenAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
        }

        public async Task<RefreshToken> GetByValueAsync(string value)
        {
            var token = await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Token == value);

            return token;
        }

        public async Task<RefreshToken> GetRefteshTokenByUserId(Guid userId)
        {
            var token = await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
            return token;
        }

        public void RemoveRefreshToken(RefreshToken token)
        {
            _context.RefreshTokens.Remove(token);

        }
    }
}
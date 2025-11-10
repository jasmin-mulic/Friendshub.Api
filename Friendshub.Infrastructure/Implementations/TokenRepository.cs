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

        public async Task<RefreshToken> GetRefreshTokenByValue(string value)
        {
            return await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Token == value);
        }

        public async Task<RefreshToken> GetRefreshTokenByUserId(Guid userId)
        {
            return await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public void RemoveRefreshToken(RefreshToken token)
        {
            _context.RefreshTokens.Remove(token);
        }
    }
}
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<RefreshToken> GetRefteshTokenByUserId(Guid userId)
        {
            return await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}

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

        public async Task<RefreshToken> AddRefreshToken(Guid userId)
        {
            var refreshToken = new RefreshToken()
            {
                Id = new Guid(),
                UserId = userId,
                Token = CreateRefreshToken(),
                ExpiresOnUtc = DateTime.UtcNow.AddDays(1)
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            return refreshToken;
        }

        public async Task<string> CreateAccessToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
            var expiryInMinutes = Convert.ToInt32(jwtSettings["ExpiryInMinutes"]);
            
            var roles = await _context.UserRoles.Include("Role").Where(x => x.UserId == user.Id).ToListAsync();
            var claims = new List<Claim>();
            foreach (var userRole in roles)
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.DateOfBirth, user.DateOfBirth.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, user.EmailAddress));
            claims.Add(new Claim(ClaimTypes.Name, user.DisplayUsername));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(2),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        public void DeleteRefreshToken(Guid UserId)
        {
            var token = _context.RefreshTokens.FirstOrDefault(x => x.UserId == UserId);
            _context.RefreshTokens.Remove(token);
        }

        public async Task<RefreshToken> GetRefreshTokenByValue(string value)
        {
            return  await _context.RefreshTokens.FirstOrDefaultAsync(rToken => rToken.Token == value);


        }

        public async Task<RefreshToken> GetUserRefreshToken(User user)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(x =>  x.UserId == user.Id);
            return token;
        }
    }
}

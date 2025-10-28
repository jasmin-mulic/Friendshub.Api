using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Friendshub.Application.Interfaces.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public TokenService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.TokenRepository.AddRefreshTokenAsync(refreshToken);
            await _unitOfWork.ApplyChangesAsync();
            return refreshToken;
        }

        public async Task<string> CreateAccessToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
            var expiryInMinutes = Convert.ToInt32(jwtSettings["ExpiryInMinutes"]);

            var roles = await _unitOfWork.UserRoleRepository.GetRolesByUserId(user.Id);
            var claims = new List<Claim>();
            foreach (var userRole in roles)
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.DateOfBirth, user.DateOfBirth.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, user.EmailAddress));
            claims.Add(new Claim(ClaimTypes.Name, user.Username));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(20),
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

        public async Task DeleteRefreshToken(Guid userId)
        {
            var token = await _unitOfWork.TokenRepository.GetRefteshTokenByUserId(userId);
            await _unitOfWork.TokenRepository.RemoveRefreshToken(token);
        }

        public Task<RefreshToken> GetRefreshTokenByValue(string value)
        {
            return _unitOfWork.TokenRepository.GetByValueAsync(value); ;
        }
    }
}

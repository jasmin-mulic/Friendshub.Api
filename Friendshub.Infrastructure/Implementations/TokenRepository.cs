using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Friendshub.Infrastructure.Implementations
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public TokenRepository(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;

        }

        public async Task AddRefreshTokenAsync(RefreshToken token)
        {
            await _unitOfWork.TokenRepository.AddRefreshTokenAsync(token);
        }

        public Task<string> CreateAccessToken(User user)
        {
            return _unitOfWork.TokenRepository.CreateAccessToken(user);
        }

        public async Task<RefreshToken> GetRefreshTokenByValue(string value)
        {
            return await _unitOfWork.TokenRepository.GetRefreshTokenByValue(value);
        }

        public async Task<RefreshToken> GetRefreshTokenByUserId(Guid userId)
        {
            return await _unitOfWork.TokenRepository.GetRefreshTokenByUserId(userId);
        }

        public void RemoveRefreshToken(RefreshToken token)
        {
            _unitOfWork.TokenRepository.RemoveRefreshToken(token);
        }
    }
}
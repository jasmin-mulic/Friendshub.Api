using Friendshub.Domain.Models;
namespace Friendshub.Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(User user);
        string CreateRefreshToken();
        Task<RefreshToken> GetUserRefreshToken(Guid userId);
        Task<RefreshToken> AddRefreshToken(Guid userId);
        Task<RefreshToken> GetRefreshTokenByValue(string value);
        Task DeleteRefreshTokenByUserId(Guid userId);
    }
}

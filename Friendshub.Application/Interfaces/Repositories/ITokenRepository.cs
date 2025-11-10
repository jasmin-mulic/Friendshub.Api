using Friendshub.Domain.Models;
namespace Friendshub.Application.Interfaces.Repositories
{
    public interface ITokenRepository
    {
        Task AddRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken> GetRefreshTokenByUserId(Guid userId);
        void RemoveRefreshToken(RefreshToken token);
        Task<RefreshToken> GetRefreshTokenByValue (string value);
        
    }
}

using Friendshub.Domain.Models;
namespace Friendshub.Application.Interfaces.Repositories
{
    public interface ITokenRepository
    {
        Task AddRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken> GetRefteshTokenByUserId(Guid userId);

        void RemoveRefreshToken(RefreshToken token);
        Task<RefreshToken> GetByValueAsync (string value);
        
    }
}

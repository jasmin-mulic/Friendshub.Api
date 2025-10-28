using Friendshub.Domain.Models;
namespace Friendshub.Application.Repositories
{
    public interface ITokenRepository
    {
        Task AddRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken> GetRefteshTokenByUserId(Guid userId);
        Task RemoveRefreshToken(RefreshToken token);
        Task<RefreshToken> GetByValueAsync (string value);
        
    }
}

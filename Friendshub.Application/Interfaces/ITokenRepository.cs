using Friendshub.Domain.Models;
namespace Friendshub.Application.Repositories
{
    public interface ITokenRepository
    {
        Task AddRefreshTokenAsync(RefreshToken token);
        RefreshToken GetRefteshTokenByUserId(Guid userId);
        Task RemoveRefreshToken(RefreshToken token);
        
    }
}

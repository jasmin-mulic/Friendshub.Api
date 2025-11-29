using Friendshub.Application.Features.Auth.DTO;
using Friendshub.Application.Results;

namespace Friendshub.Application.Features.Auth
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginUserDto request);
        Task<RegisterResult> RegisterAsync(RegisterUserDto request);
        Task<bool> DeleteAccountAsync(Guid userId, string password);
    }
}

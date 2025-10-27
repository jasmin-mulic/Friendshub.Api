
using Friendshub.Application.DTO.Auth;
using Friendshub.Application.Results;

namespace Friendshub.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginUserDto request);
        Task<RegisterResult> RegisterAsync(RegisterUserDto request);
        Task<bool> DeleteAccountAsync(Guid userId, string password);
    }
}

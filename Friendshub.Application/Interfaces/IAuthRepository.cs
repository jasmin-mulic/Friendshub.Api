using Friendshub.Application.DTO.Auth;
using Friendshub.Application.Results;

namespace Friendshub.Application.Repositories
{
    public interface IAuthRepository
    {
        Task<LoginResult> LoginAsync(LoginUserDto request);
        Task<RegisterResult> RegisterAsync(RegisterUserDto request);
        Task<bool> DeleteAccount(Guid userId, string password);
    }
}

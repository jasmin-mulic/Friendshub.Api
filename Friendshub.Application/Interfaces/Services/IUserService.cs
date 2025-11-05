using Friendshub.Application.DTO.UserDto;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<LoggedUserData> GetMyProfileData(User user);
        Task<User> GetByIdAsync(Guid id);
        Task<Dictionary<string, string>> UpdateUserData(Guid id, UpdateUserInfoDto updateUserInfo);
        Task<UserProfileData> GetUserProfileData(string username);
        Task<User> GetUserByEmailOrUsername(string emailOrUsername);
        Task<bool> IsUsernameTaken(string username);
        Task<bool> IsEmailAddressTaken(string emailAddress);
        Task AddAsync(User user);
        void DeleteUser(User user);
        Task<User> GetByIdAsNoTracking(Guid id);
    }
}

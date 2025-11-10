using Friendshub.Application.DTO.UserDto;
using Friendshub.Domain.Models;
namespace Friendshub.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserById(Guid id);
        void UpdateUserInfo(User user);
        Task<User> GetUserByEmailOrUsername(string emailOrUsername);
        Task<bool> IsUsernameTaken(string username);
        Task<bool> IsEmailAddressTaken(string emailAddress);
        Task AddAsync(User user);
        void DeleteUser(User user);
        Task<User> GetByIdAsNoTracking(Guid id);
    }
}

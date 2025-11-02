using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByIdAsNoTracking(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(id);
            return user;
        }

        public Task<MyProfileData> GetMyProfileData(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByEmailOrUsername(string emailOrUsername)
        {
            throw new NotImplementedException();
        }

        public Task<UserProfileData> GetUserProfileData(string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsEmailAddressTaken(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUsernameTaken(string username)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> UpdateUserData(Guid id, UpdateUserInfoDto updateUserInfo)
        {
            throw new NotImplementedException();
        }
    }
}

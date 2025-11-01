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
        public async Task<User> GetByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(id);
            return user;
        }
    }
}

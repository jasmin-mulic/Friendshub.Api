using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(Guid id);
    }
}

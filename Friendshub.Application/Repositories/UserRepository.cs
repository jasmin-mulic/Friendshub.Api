using Friendshub.Application.DTO.User;
using Friendshub.Domain.Models;
using Microsoft.AspNetCore.Http;
namespace Friendshub.Application.Repositories
{
    public interface IUserRepository
    {
        Task<ProfileDataDto> GetProfileData(User user);
        Task<User> GetById(Guid id);
        Task<string> ChangeProfilePicture(IFormFile file);
        Task <List<FollowRecommendation>> GetFollowRecommendationList(Guid id);
        void FollowUser(Guid folower, Guid Folowee);
    }
}

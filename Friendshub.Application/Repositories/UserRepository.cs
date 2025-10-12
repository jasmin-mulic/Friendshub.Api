
using Friendshub.Application.DTO.UserDto;
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
        Task<string> FollowUser(Guid folower, Guid Folowee);
        Task  DeleteUser(Guid id);
        Task<List<UserBasicInfo>> GetFollowers(Guid userId);
    }
}

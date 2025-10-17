
using Friendshub.Application.DTO.UserDto;
using Friendshub.Domain.Models;
using Microsoft.AspNetCore.Http;
namespace Friendshub.Application.Repositories
{
    public interface IUserRepository
    {
        Task<ProfileDataDto> GetProfileData(User user);
        Task<User> GetUserById(Guid id);
        Task<string> ChangeProfilePicture(Guid userId, IFormFile file);
        Task <List<FollowRecommendationDto>> GetFollowRecommendationList(Guid id);
        Task<string> FollowUser(Guid folower, Guid Folowee);
        Task  DeleteUser(Guid id);
        Task<List<UserBasicInfo>> GetFollowers(Guid userId);
        void RemoveFollower(Guid followeeId, Guid followerId);
        Task<List<UserBasicInfo>> GetFollowings(Guid userId);
        Task<UpdateUserValidationDto> UpdateUserInfo(Guid id, UpdateUserInfoDto updateUserInfo);
    }
}

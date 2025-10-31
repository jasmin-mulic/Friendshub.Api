using Friendshub.Application.DTO.UserDto;
using Friendshub.Domain.Models;
using Microsoft.AspNetCore.Http;
namespace Friendshub.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<MyProfileData> GetMyProfileData(User user);
        Task<User> GetUserById(Guid id);
        Task <List<FollowRecommendationDto>> GetFollowRecommendationList(Guid id);
        Task<string> FollowUser(Guid folower, Guid Folowee);
        Task<List<UserBasicInfo>> GetFollowers(Guid userId);
        void RemoveFollower(Guid followeeId, Guid followerId);
        Task<List<UserBasicInfo>> GetFollowings(Guid userId);
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

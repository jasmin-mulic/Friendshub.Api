using Friendshub.Application.DTO;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Domain.Models;
namespace Friendshub.Application.Interfaces.Repositories
{
    public interface IFollowRepository
    {
        Task<List<UserBasicInfo>> GetUserFollowingsList(Guid userId);
        Task<List<UserBasicInfo>> GetUserFollowersList(Guid userId);
        void DeleteFollow(Follow follow);
        Task<List<Guid>> GetFollowingUsersIds(Guid followerId);
        Task<Follow> GetByIdAsync(Guid followerId, Guid foloweeId);
        void RemoveFollows(List<Follow> follows);
        Task AddFollowAsync(Follow follow);
        Task<List<User>> GetFollowRecommendations(Guid userId, int skip, int take;
        Task<int> GetFollowRecommendationsCountAsync(Guid userId, int skip, int take);
    }
}

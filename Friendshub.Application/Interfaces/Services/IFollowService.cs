using Friendshub.Application.DTO;
using Friendshub.Application.Features.Users.DTO;

namespace Friendshub.Application.Interfaces.Services
{
    public interface IFollowService
    {
        Task<List<UserBasicInfo>> GetFollowers(Guid userId);
        Task<List<UserBasicInfo>> GetUserFollowingsList(Guid userId);
        Task<string> AddFollowAsync(Guid folower, Guid Followee);
        Task<bool> RemoveFollowAsync(Guid follower, Guid Followee);
        Task RemoveFromFollows(Guid followerId, Guid followeeId);
        Task RemoveFromFollowers(Guid followerId, Guid followeeId);
        Task<PageResult<FollowRecommendationDto>> GetFollowRecommendationList(Guid id, int pageNumber);



    }
}

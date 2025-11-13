using Friendshub.Application.DTO;
using Friendshub.Application.DTO.UserDto;

namespace Friendshub.Application.Interfaces.Services
{
    public interface IFollowService
    {
        Task<List<UserBasicInfo>> GetUserFollowersList(Guid userId);
        Task<List<UserBasicInfo>> GetUserFollowingsList(Guid userId);
        Task<string> AddFollowAsync(Guid folower, Guid Folowee);
        Task RemoveFromFollows(Guid followerId, Guid followeeId);
        Task RemoveFromFollowers(Guid followerId, Guid followeeId);
        Task<PageResult<FollowRecommendationDto>> GetFollowRecommendationList(Guid id, int pageNumber);



    }
}

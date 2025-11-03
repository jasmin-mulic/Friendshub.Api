using Friendshub.Domain.Models;
namespace Friendshub.Application.Interfaces.Repositories
{
    public interface IFollowRepository
    {
        Task<List<User>> GetFollowingUsersList(Guid userId);
        Task<List<Follow>> GetFollowersUserList(Guid userId);
        Task<List<User>> GetUserFollowersList(Guid userId);
        void DeleteFollow(Follow follow);
        Task<List<Guid>> GetFollowingUsersIds(Guid followerId);
        Task<Follow> GetByIdAsync(Guid followerId, Guid foloweeId);
        void RemoveFollows(List<Follow> follows);
        Task AddFollowAsync(Follow follow);
        Task<List<User>> GetFollowRecommendationsAsync(Guid userId, int skip, int take);
        Task<int> GetFollowRecommendationsCountAsync(Guid userId);
    }
}

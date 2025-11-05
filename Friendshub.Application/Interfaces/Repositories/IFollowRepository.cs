using Friendshub.Domain.Models;
namespace Friendshub.Application.Interfaces.Repositories
{
    public interface IFollowRepository
    {
        Task<List<User>> GetUserFollowings(Guid userId);
        Task<List<Follow>> GetUserFollowingList(Guid userId);
        Task<List<User>> GetUserFollowers(Guid userId);
        void DeleteFollow(Follow follow);
        Task<List<Guid>> GetFollowingUsersIds(Guid followerId);
        Task<Follow> GetByIdAsync(Guid followerId, Guid foloweeId);
        void RemoveFollows(List<Follow> follows);
        Task AddFollowAsync(Follow follow);
        Task<List<User>> GetFollowRecommendationsAsync(Guid userId, int skip, int take);
        Task<int> GetFollowRecommendationsCountAsync(Guid userId);
        Task<int> GetUserFollowersCount(Guid userId);
        Task<int> GetFollowingCount(Guid userId);
    }
}

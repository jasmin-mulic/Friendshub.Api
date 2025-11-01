using Friendshub.Domain.Models;
namespace Friendshub.Application.Interfaces.Repositories
{
    public interface IFollowRepository
    {
        Task<List<Follow>> GetUserFollowingList(Guid userId);
        void DeleteFollow(Follow follow);
        Task<List<Guid>> GetFollowingUsersIds(Guid followerId);
        Task<Follow> GetByIdAsync(Guid followerId, Guid foloweeId);

    }
}

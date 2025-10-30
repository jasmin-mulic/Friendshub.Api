using Friendshub.Domain.Models;
namespace Friendshub.Application.Interfaces.Repositories
{
    public interface IFollowRepository
    {
        Task<List<Follow>> GetUserFollowingList(Guid userId);
        void RemoveFollows(List<Guid> follows);


    }
}

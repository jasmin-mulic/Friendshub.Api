using Friendshub.Domain.Models;
namespace Friendshub.Application.Interfaces
{
    public interface IFollowRepository
    {
        Task<List<Follow>> GetUserFollowingList(Guid userId);
        void RemoveFollows(List<Follow> follows);


    }
}

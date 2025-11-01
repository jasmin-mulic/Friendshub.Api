using Friendshub.Application.DTO.UserDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Interfaces.Services
{
    public interface IFollowService
    {
        Task<List<UserBasicInfo>> GetFollowers(Guid userId);
        Task<List<UserBasicInfo>> GetFollowings(Guid userId);
        Task<string> FollowUser(Guid folower, Guid Folowee);
        void RemoveFollower(Guid followeeId, Guid followerId);
        Task<List<FollowRecommendationDto>> GetFollowRecommendationList(Guid id);



    }
}

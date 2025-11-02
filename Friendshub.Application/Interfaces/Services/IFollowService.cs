using Friendshub.Application.DTO;
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
        Task<List<UserBasicInfo>> GetUserFollowersList(Guid userId);
        Task<List<UserBasicInfo>> GetUserFollowingsList(Guid userId);
        Task<string> FollowUser(Guid folower, Guid Folowee);
        void RemoveFollower(Guid followeeId, Guid followerId);
        Task<PageResult<FollowRecommendationDto>> GetFollowRecommendationList(Guid id);



    }
}

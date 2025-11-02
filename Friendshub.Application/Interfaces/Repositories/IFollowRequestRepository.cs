using Friendshub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Interfaces.Repositories
{
    public interface IFollowRequestRepository
    {
        Task<FollowRequest> GetPendingRequest(Guid senderId, Guid recieverId);
        void RemoveFollowRequest(FollowRequest request);
        Task AddFollowRequest(FollowRequest request);
    }
}

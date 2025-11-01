using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Infrastructure.Implementations
{
    public class FollowRequestRepository : IFollowRequestRepository
    {
        private readonly FriendshubDbContext _context;
        public FollowRequestRepository(FriendshubDbContext context)
        {
            _context = context;
        }
        public async Task<FollowRequest> GetPendingRequest(Guid senderId, Guid recieverId)
        {
            return await _context.FollowRequests.FirstOrDefaultAsync(x => x.SenderId == senderId && x.RecieverId == recieverId);    
        }

        public void RemoveFollowRequest(FollowRequest followRequest)
        {
            _context.FollowRequests.Remove(followRequest);

        }
    }
}

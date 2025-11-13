using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Friendshub.Infrastructure.Implementations
{
    public class FollowRequestRepository : IFollowRequestRepository
    {
        private readonly FriendshubDbContext _context;
        public FollowRequestRepository(FriendshubDbContext context)
        {
            _context = context;
        }

        public async Task AddFollowRequest(FollowRequest request)
        {
            await _context.AddAsync(request);
        }

        public async Task<FollowRequest> GetPendingRequest(Guid senderId, Guid recieverId)
        {
            return await _context.FollowRequests.FirstOrDefaultAsync(x => x.SenderId == senderId && x.RecieverId == recieverId);    
        }

        public async Task<List<Guid>> GetUserSentRequest(Guid userId)
        {
            return await _context.FollowRequests.AsNoTracking().Where(x =>x.SenderId == userId).Select(x => x.RecieverId).ToListAsync();
        }

        public async Task<bool> PendingRequestExists(Guid senderId, Guid recieverId)
        {
            return await _context.FollowRequests.AnyAsync(x => x.SenderId == senderId && x.RecieverId == recieverId);

        }

        public void RemoveFollowRequest(FollowRequest followRequest)
        {
            _context.FollowRequests.Remove(followRequest);

        }
    }
}

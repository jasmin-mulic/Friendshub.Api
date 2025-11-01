using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Implementations
{
    public class FollowService : IFollowService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FollowService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<string> FollowUser(Guid folowerId, Guid foloweeId)
        {
            var existingFollow = await _unitOfWork.FollowRepository.GetByIdAsync(followerId, foloweeId);

            if (existingFollow != null)
            {
                _unitOfWork.FollowRepository.DeleteFollow(existingFollow);
                return "unfollowed";
            }

            var pendingRequest = await _context.FollowRequests
                .FirstOrDefaultAsync(x => x.SenderId == followerId && x.RecieverId == followeeId);

            if (pendingRequest != null)
            {
                _context.FollowRequests.Remove(pendingRequest);
                return "Follow request canceled.";
            }

            var followee = await _context.Users.FirstOrDefaultAsync(x => x.Id == followeeId);
            if (followee == null)
                throw new ApplicationException("Followee not found.");

            if (!followee.PrivateAccount)
            {
                await _context.Follows.AddAsync(new Follow
                {
                    FollowerId = followerId,
                    FolloweeId = followeeId
                });
                return "followed";
            }

            await _context.FollowRequests.AddAsync(new FollowRequest
            {
                SenderId = followerId,
                RecieverId = followeeId
            });
            return "Follow request sent";
        }

        public Task<List<UserBasicInfo>> GetFollowers(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserBasicInfo>> GetFollowings(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<FollowRecommendationDto>> GetFollowRecommendationList(Guid id)
        {
            throw new NotImplementedException();
        }

        public void RemoveFollower(Guid followeeId, Guid followerId)
        {
            throw new NotImplementedException();
        }
    }
}

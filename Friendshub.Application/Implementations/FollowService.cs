using Friendshub.Application.DTO;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Implementations
{
    public class FollowService : IFollowService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FollowService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<string> FollowUser(Guid followerId, Guid followeeId)
        {
            var existingFollow = await _unitOfWork.FollowRepository.GetByIdAsync(followerId, followeeId);

            if (existingFollow != null)
            {
                _unitOfWork.FollowRepository.DeleteFollow(existingFollow);
                return "unfollowed";
            }
            var followee = await _unitOfWork.UserRepository.GetByIdAsNoTracking(followeeId) ?? throw new ApplicationException("Followee not found.");

            if (followee.PrivateAccount)
            {
                var pendingRequest = await _unitOfWork.FollowRequestRepository.GetPendingRequest(followerId, followeeId);

                if (pendingRequest != null)
                {
                    _unitOfWork.FollowRequestRepository.RemoveFollowRequest(pendingRequest);
                    return "Follow request canceled.";
                }
                var followRequest = new FollowRequest
                {
                    SenderId = followerId,
                    RecieverId = followeeId
                };
                await _unitOfWork.FollowRequestRepository.AddFollowRequest(followRequest);
                return "Follow request sent.";
            }
            var newFollow = new Follow
            {
                FolloweeId = followeeId,
                FollowerId = followerId,
            };
            await _unitOfWork.FollowRepository.AddFollowAsync(newFollow);

            return "followed";
        }

        public async Task<List<UserBasicInfo>> GetUserFollowersList(Guid userId)
        {
            return await _unitOfWork.FollowRepository.GetUserFollowersList(userId);
        }

        public async Task<List<UserBasicInfo>> GetUserFollowingsList(Guid userId)
        {
            return await _unitOfWork.FollowRepository.GetUserFollowingsList(userId);
        }

        public async Task<PageResult<FollowRecommendationDto>> GetFollowRecommendationList(Guid userId, int pageNumber, int pageSize = 10)
        {
            var recommendationsPage = await _unitOfWork.FollowRepository.GetFollowRecommendations(userId, pageNumber, pageSize);
        }

        public void RemoveFollower(Guid followeeId, Guid followerId)
        {
            throw new NotImplementedException();
        }
    }
}

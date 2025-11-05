using Friendshub.Application.DTO;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Extensions;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
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
        public async Task<string> AddFollowAsync(Guid followerId, Guid followeeId)
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
            var followers =  await _unitOfWork.FollowRepository.GetUserFollowers(userId);

            var followersList = followers.Select(f => new UserBasicInfo
            {
                UserId = f.Id,
                Username = f.Username,
                ProfileImageUrl = f.ProfileImageUrl?.ToFullImageUrl(),
            }).ToList();
            return followersList;
        }

        public async Task<List<UserBasicInfo>> GetUserFollowingsList(Guid userId)
        {
            var followings = await _unitOfWork.FollowRepository.GetUserFollowings(userId);

            var followingsList = followings.Select(f => new UserBasicInfo
            {
                UserId = f.Id,
                Username = f.Username,
                ProfileImageUrl = f.ProfileImageUrl?.ToFullImageUrl(),
            }).ToList();
            return followingsList;
        }

        public async Task<PageResult<UserBasicInfo>> GetFollowRecommendationList(Guid userId, int pageNumber, int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;

            var skip = (pageNumber - 1) * pageSize;
            var totalCount = await _unitOfWork.FollowRepository.GetFollowRecommendationsCountAsync(userId);
            var users = await _unitOfWork.FollowRepository.GetFollowRecommendationsAsync(userId, skip, pageSize);

            var recommendations = users.Select(u => new UserBasicInfo
            {
                UserId = u.Id,
                Username = u.Username,
                ProfileImageUrl = u.ProfileImageUrl?.ToFullImageUrl(),
            }).ToList();

            var pageResult = new PageResult<UserBasicInfo>
            {
                PageNumber = pageNumber,
                TotalCount = totalCount,
                Items = recommendations,
                PageSize = pageSize
            };
            return pageResult;
        }

        public async Task RemoveFromFollows(Guid followerId, Guid followeeId)
        {
            var follow = await _unitOfWork.FollowRepository.GetByIdAsync(followerId, followeeId);
            if (follow == null)
            {
                throw new NullReferenceException("You are not following user.");
            }
            _unitOfWork.FollowRepository.DeleteFollow(follow);
            await _unitOfWork.ApplyChangesAsync();
        }

        public async Task RemoveFromFollowers(Guid followerId, Guid followeeId)
        {
            var follow = await _unitOfWork.FollowRepository.GetByIdAsync(followerId, followeeId);
        }
    }
}

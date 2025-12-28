using Friendshub.Application.DTO;
using Friendshub.Application.Extensions;
using Friendshub.Application.Features.Users.DTO;
using Friendshub.Application.Interfaces;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using System.Threading;

namespace Friendshub.Application.Implementations
{
    public class FollowService : IFollowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly INotificationSender _notificationSender;
        public FollowService(IUnitOfWork unitOfWork, INotificationService notificationService,INotificationSender notificationSender)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _notificationSender = notificationSender;
        }
        public async Task<string> AddFollowAsync(Guid followerId, Guid followeeId)
        {
            // TODO: Missing transactions
            try
            {
                var existingFollow = await _unitOfWork.FollowRepository.GetByIdAsync(followerId, followeeId);

                if (existingFollow is not null)
                {
                    _unitOfWork.FollowRepository.RemoveFollow(existingFollow);
                    await _unitOfWork.ApplyChangesAsync();
                    return "Unfollowed";
                }
                var followee = await _unitOfWork.UserRepository.GetByIdAsNoTracking(followeeId) ?? throw new ApplicationException("Followee not found.");
                var follower = await _unitOfWork.UserRepository.GetByIdAsNoTracking(followerId) ?? throw new ApplicationException("Followee not found.");


                if (followee.PrivateAccount)
                {
                    var pendingRequest = await _unitOfWork.FollowRequestRepository.GetPendingRequest(followerId, followeeId);

                    if (pendingRequest != null)
                    {
                        _unitOfWork.FollowRequestRepository.RemoveFollowRequest(pendingRequest);
                        await _unitOfWork.ApplyChangesAsync();
                        return "Follow request canceled";
                    }
                    var followRequest = new FollowRequest
                    {
                        SenderId = followerId,
                        RecieverId = followeeId
                    };
                    var notification = new Notification()
                    {
                        Id = Guid.NewGuid(),
                        NotificationType = NotificationType.Like,
                        CreatedAt = DateTime.Now,
                        ReceiverId = followeeId,
                        SenderId = followerId,
                        Message = follower.Username + " sent you follow request",
                        EntityId = null,
                        IsOpened = false,
                    };
                    await _unitOfWork.NotificationRepository.AddNotificationAsync(notification);
                    await _unitOfWork.FollowRequestRepository.AddFollowRequest(followRequest);
                    await _notificationSender.SendAsync(followeeId, notification);
                    await _unitOfWork.ApplyChangesAsync();

                    return "Follow request sent";
                }
                var newFollow = new Follow
                {
                    FolloweeId = followeeId,
                    FollowerId = followerId,
                };
                await _unitOfWork.FollowRepository.AddFollowAsync(newFollow);
                await _unitOfWork.ApplyChangesAsync();
                return "Followed";
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<List<UserBasicInfo>> GetFollowers(Guid userId)
        {
            var followers =  await _unitOfWork.FollowRepository.GetUserFollowers(userId);

            var followersList = followers.Select(f => new UserBasicInfo
            {
                UserId = f.Id,
                Username = f.Username,
                ProfileImageUrl = f.ProfileImageUrl?.ToFullImagePath(),
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
                ProfileImageUrl = f.ProfileImageUrl?.ToFullImagePath(),
            }).ToList();
            return followingsList;
        }

        public async Task<PageResult<FollowRecommendationDto>> GetFollowRecommendationList(Guid userId, int pageNumber)
        {
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;

            var skip = (pageNumber - 1) * pageSize;
            var totalCount = await _unitOfWork.FollowRepository.GetFollowRecommendationsCountAsync(userId);
            var users = await _unitOfWork.FollowRepository.GetFollowRecommendationsAsync(userId, skip, pageSize);

            var userIds = users.Select(u => u.Id).ToList();

            var pendingRequests = await _unitOfWork.FollowRequestRepository.GetUserSentRequest(userId);

            var recommendations = users.Select(u => new FollowRecommendationDto
            {
                UserId = u.Id,
                Username = u.Username,
                ProfileImageUrl = u.ProfileImageUrl?.ToFullImagePath(),
                PendingRequest = pendingRequests.Contains(u.Id)
            }).ToList();

            return new PageResult<FollowRecommendationDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = recommendations
            };
        }


        public async Task RemoveFromFollows(Guid followerId, Guid followeeId)
        {
            var follow = await _unitOfWork.FollowRepository.GetByIdAsync(followerId, followeeId);
            if (follow == null)
            {
                throw new NullReferenceException("You are not following user.");
            }
            _unitOfWork.FollowRepository.RemoveFollow(follow);
            await _unitOfWork.ApplyChangesAsync();
        }

        public async Task RemoveFromFollowers(Guid followeeId, Guid followerId)
        {
            var follow = await _unitOfWork.FollowRepository.GetByIdAsync(followeeId, followerId);
            _unitOfWork.FollowRepository.RemoveFollow(follow);
            await _unitOfWork.ApplyChangesAsync();
        }

        public async Task<bool> RemoveFollowAsync(Guid followerId, Guid followeeId)
        {
            var existingFollow = await _unitOfWork.FollowRepository.GetByIdAsync(followeeId, followerId);
            if(existingFollow is not null)
            {
                _unitOfWork.FollowRepository.RemoveFollow(existingFollow);
                await _unitOfWork.ApplyChangesAsync();
                return true;
            }
            return false;
        }
    }
}

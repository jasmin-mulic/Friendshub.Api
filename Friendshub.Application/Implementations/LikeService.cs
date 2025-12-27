using Friendshub.Application.Extensions;
using Friendshub.Application.Features.Posts.DTO;
using Friendshub.Application.Features.Users.DTO;
using Friendshub.Application.Interfaces;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Implementations
{
    public class LikeService : ILIkeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationSender _notificationSender;
        

        public LikeService(IUnitOfWork unitOfWork, INotificationSender notificationSender )
        {
            _unitOfWork = unitOfWork;
            _notificationSender = notificationSender;
        }

        public async Task<LikeCommentResponseDto> LikePostComment(Guid commentId, Guid userId)
        {
            var response = new LikeCommentResponseDto();
            var alreadyLiked = await _unitOfWork.CommentLikeRepository.GetUserLikeAsync(userId,commentId);
            if (alreadyLiked != null)
            {
                response.CommentId = commentId;
                response.User.UserId = userId;
                response.IsLiked = true;
                _unitOfWork.CommentLikeRepository.RemoveCommentLike(alreadyLiked);
                await _unitOfWork.ApplyChangesAsync();
                return response;
            }
            var user = await _unitOfWork.UserRepository.GetByIdAsNoTracking(userId);

            response.CommentId = commentId;
            response.User.ProfileImageUrl = user.ProfileImageUrl;
            response.User.Username = user.Username;
            response.User.ProfileImageUrl = user.ProfileImageUrl ?? null;
            response.User.UserId = userId;
            response.IsLiked = true;

            var newLike = new CommentLike()
            {
                UserId = userId, 
                CommentId = commentId,
            };
            await _unitOfWork.CommentLikeRepository.AddCommentLike(newLike);
            await _unitOfWork.ApplyChangesAsync();
            return response;  
        }
        public async Task<bool> LikePost(Guid userId, Guid postId)
        {
            var usersPostLike = await _unitOfWork.PostLikeRepository.GetPostLikeForUser(postId, userId);

            if (usersPostLike != null)
            {
                _unitOfWork.PostLikeRepository.RemoveLike(usersPostLike);
                var existingNotification = await _unitOfWork.NotificationRepository.GetNotificationByPostId(postId);
                if (existingNotification != null)
                    _unitOfWork.NotificationRepository.DeleteNotification(existingNotification);
                await _unitOfWork.ApplyChangesAsync();
                return false;
            }
            var newLike = new PostLike
            {
                UserId = userId,
                PostId = postId,
                LikedAt = DateTime.Now,
            };
            await _unitOfWork.PostLikeRepository.AddLike(newLike);

            var userLiked = await _unitOfWork.UserRepository.GetByIdAsNoTracking(userId);
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
            var notification = new Notification()
            {
                Id = Guid.NewGuid(),
                NotificationType = NotificationType.Like,
                CreatedAt = DateTime.Now,
                ReceiverId = post.UserId,
                SenderId = userId,
                Message = userLiked.Username + " liked your post",
                EntityId = post.Id,
                isRead = false,
            };
            await _unitOfWork.NotificationRepository.AddNotificationAsync(notification);
            await _unitOfWork.ApplyChangesAsync();
            if(post.UserId != userId)
                await _notificationSender.SendAsync(post.UserId, notification);
            return true;
        }

        public async Task<List<UserBasicInfo>> GetPostLikes(Guid PostId)
        {
            var likes = await _unitOfWork.PostLikeRepository.GetPostLikes(PostId);
            if (likes == null || likes.Count == 0)
                return null;
            var postLikesDto = likes.Select(postLike => new UserBasicInfo
            {
                ProfileImageUrl = postLike.User.ProfileImageUrl?.ToFullImagePath(),
                UserId = postLike.UserId,
                Username = postLike.User.Username,

            }).ToList();
            return postLikesDto;
        }
    }
}

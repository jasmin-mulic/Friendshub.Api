using Friendshub.Application.DTO.PostDto;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Extensions;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Implementations
{
    public class LikeService : ILIkeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public LikeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LikeCommentResponseDto> LikePostComment(Guid commentId, Guid userId)
        {
            var response = new LikeCommentResponseDto();
            var existingLike = await _unitOfWork.CommentLikeRepository.GetUserLikeAsync(userId,commentId);
            if (existingLike != null)
            {
                response.CommentId = commentId;
                response.Message = "disliked";
                response.User.UserId = userId;
                _unitOfWork.CommentLikeRepository.RemoveCommentLike(existingLike);
                await _unitOfWork.ApplyChangesAsync();
                return response;
            }
            var user = await _unitOfWork.UserRepository.GetByIdAsNoTracking(userId);

            response.Message = "liked";
            response.CommentId = commentId;
            response.User.ProfileImageUrl = user.ProfileImageUrl;
            response.User.Username = user.Username;
            response.User.ProfileImageUrl = user.ProfileImageUrl ?? null;
            response.User.UserId = userId;

            var newLike = new CommentLike()
            {
                UserId = userId, 
                CommentId = commentId,
            };
            await _unitOfWork.CommentLikeRepository.AddCommentLike(newLike);
            await _unitOfWork.ApplyChangesAsync();
            return response;  
        }
        public async Task<string> LikePost(Guid userId, Guid postId)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
            if (post == null)
                throw new NullReferenceException("Post not found.");
            var postLikes = await _unitOfWork.PostLikeRepository.GetPostLikes(postId);
            var myLike = postLikes.FirstOrDefault(x => x.UserId == userId);

            if (myLike != null)
            {
                _unitOfWork.PostLikeRepository.RemoveLike(myLike);
               await _unitOfWork.ApplyChangesAsync();
                return "Disliked";
            }
            var newLike = new PostLike
            {
                UserId = userId,
                PostId = postId,
                LikedAt = DateTime.Now,
            };
            await _unitOfWork.PostLikeRepository.AddLike(newLike);

            await _unitOfWork.ApplyChangesAsync();

            return "Liked";
        }

        public async Task<List<UserBasicInfo>> GetPostLikes(Guid PostId)
        {
            var likes = await _unitOfWork.PostLikeRepository.GetPostLikes(PostId);
            if (likes == null || likes.Count == 0)
                return null;
            var postLikesDto = likes.Select(postLike => new UserBasicInfo
            {
                ProfileImageUrl = postLike.User.ProfileImageUrl?.ToFullImageUrl(),
                UserId = postLike.UserId,
                Username = postLike.User.Username,

            }).ToList();
            return postLikesDto;
        }
    }
}

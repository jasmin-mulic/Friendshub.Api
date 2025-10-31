using Friendshub.Application.DTO.PostDto;
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
            var existingLike = await _context.CommentsLikes.FirstOrDefaultAsync((x => x.UserId == userId && x.CommentId == commentId));
            if (existingLike != null)
            {
                response.CommentId = commentId;
                response.Message = "disliked";
                response.User.UserId = userId;
                _context.CommentsLikes.Remove(existingLike);
                return response;
            }
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
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
            await _context.CommentsLikes.AddAsync(newLike);
            return response;  
        }
    }
}

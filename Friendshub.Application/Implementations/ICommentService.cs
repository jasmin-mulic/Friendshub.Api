using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
using Friendshub.Application.Extensions;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<CommentClientDto> AddCommentToPost(Guid userId, Post post, AddCommentDto commentRequest)
        {
            if (string.IsNullOrWhiteSpace(commentRequest.Content) && commentRequest.Image.Length == 0)
                throw new ApplicationException("Please add a comment or an image.");
            var user = await _unitOfWork.UserRepository.GetByIdAsNoTracking(userId);

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PostId = post.Id,
                CommentedAt = DateTime.UtcNow,
                Content = commentRequest.Content,
            };

            if (commentRequest.Image != null && commentRequest.Image.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(commentRequest.Image.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    throw new ApplicationException("Invalid image format.");

                var fileName = Guid.NewGuid().ToString() + extension;
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "comments", "images").Replace("\\","/");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var physicalPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await commentRequest.Image.CopyToAsync(stream);
                }

                var relativePath = Path.Combine("uploads", "comments", "images", fileName).Replace("\\", "/");

                comment.CommentImageUrl = relativePath;
            }


            await _unitOfWork.CommentRepository.AddAsync(comment);
            await _unitOfWork.ApplyChangesAsync();
            var commentClient = new CommentClientDto
            {
                CommentImageUrl = comment.CommentImageUrl,
                CommentId = comment.Id,
                CommentedAt = DateTime.Now,
                Content = comment.Content,
                Username = user.Username,
                UserProfileImageUrl = user.ProfileImageUrl == null ? null : user.ProfileImageUrl.ToFullImageUrl(),
                UserId = user.Id,
            };
            return commentClient;
        }

        public async Task RemoveComment(Guid commentId)
        {
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(commentId);
            if (comment == null)
                throw new NullReferenceException("Comment not found");
            _unitOfWork.CommentRepository.Delete(comment);
            await _unitOfWork.ApplyChangesAsync();
        }
    }
}

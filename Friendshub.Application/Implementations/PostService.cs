using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Extensions;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Implementations
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PostService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PostClientDto> AddPost(AddPostDto request, Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(userId);
            if (user == null)
                throw new NullReferenceException("Your account is either banned or deleted.");

            if (string.IsNullOrWhiteSpace(request.Content) && (request.ImagePaths == null || request.ImagePaths.Count == 0))
                return null;

            var newPost = new Post
            {
                Id = Guid.NewGuid(),
                Content = string.IsNullOrWhiteSpace(request.Content) ? null : request.Content,
                UserId = user.Id,
            };

            if (request.ImagePaths != null && request.ImagePaths.Count > 0)
            {
                foreach (var file in request.ImagePaths)
                {
                    if (file.Length > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        var uploadsFolder = Path.Combine("wwwrooot", "uploads/posts/images");

                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var physicalPath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(physicalPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var relativePath = Path.Combine("uploads", "posts", "images", fileName).Replace("\\", "/");

                        newPost.PostsImages.Add(new PostImage
                        {
                            ImgUrl = relativePath,
                            Post = newPost
                        });
                    }
                }
            }

            await _unitOfWork.PostRepository.AddPostAsync(newPost);

            var postDto = new PostClientDto
            {
                Content = newPost.Content,
                Username = newPost.User.Username,
                PostId = newPost.Id,
                PostImagesUrl = newPost.PostsImages.Select(x => x.ImgUrl.ToFullImageUrl()).ToList(),
                PostedAt = newPost.PostedAt,
                Likes = new List<UserBasicInfo>(),
                UserId = user.Id,
            };

            return postDto;
        }

        public async Task<bool> DeletePost(Guid postId)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
            if (post == null)
                return false;
            _unitOfWork.PostRepository.DeletePost(post);
            return true;
        }

        public async Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId, int pageNumber)
        {
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 10) pageSize = 10;
            var followingsIds = await _unitOfWork.FollowRepository.GetFollowingUsersIds(userId);
            var feedPostsPage = await _unitOfWork.PostRepository.GetFeedPostsPage(userId, followingsIds, pageSize);

            return feedPostsPage;

        }

        public async Task<PageResult<PostClientDto>> GetMyPosts(Guid userId, int pageNumber)
        {
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 10) pageSize = 10;

            var pageResult = await _unitOfWork.PostRepository.GetPostsByUserIdsync(userId, pageNumber, pageSize);

            return pageResult;
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
           return await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
        }
    }
}

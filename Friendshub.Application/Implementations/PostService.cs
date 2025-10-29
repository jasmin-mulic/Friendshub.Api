using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
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
        public async Task<PostClientDto> AddPost(AddPostDto request, User user)
        {
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

        public Task<CommentClientDto> CommentPost(Guid userId, Post post, AddCommentDto comment)
        {
            throw new NotImplementedException();
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
            var usersFollowed = await _unitOfWork.FollowRepository.GetUserFollowingList(userId);
            var feedPosts = await _unitOfWork.PostRepository.GetFeedPosts(userId,)

        }

        public async Task<PageResult<PostClientDto>> GetMyPosts(Guid userId, int pageNumber)
        {
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 10) pageSize = 10;

            var posts = await _unitOfWork.PostRepository.GetPostsByUserIdsync(userId, pageNumber, pageSize);

            var PageResult = new PageResult<PostClientDto>
            {
                Items = posts,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = posts.Count,
            };
            return PageResult;
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
           return await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
        }

        public async Task<string> LikePost(Guid userId, Guid postId)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
            if (post != null)
                throw new NullReferenceException("Post not found.");
            var postLikes = await _unitOfWork.PostRepository.GetPostLikes(postId);
            var myLike = postLikes.FirstOrDefault(x => x.UserId == userId);

            if (myLike != null)
            {
                _unitOfWork.PostLikeRepository.RemoveLike(myLike);
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
    }
}

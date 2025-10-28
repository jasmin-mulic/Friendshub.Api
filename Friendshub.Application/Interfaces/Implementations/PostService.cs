using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Extensions;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Implementations
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

        public async Task DeletePost(Guid postId)
        {
            var post = await _unitOfWork.PostRepository.GetPostByIdAsync(postId);
            if (post == null)
                throw new ArgumentNullException("Post not found.");
            _unitOfWork.PostRepository.DeletePost(post);
        }

        public Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId, int page)
        {
            throw new NotImplementedException();
        }

        public async Task<PageResult<PostClientDto>> GetMyPosts(Guid userId, int pageNumber)
        {
            var posts = await _unitOfWork.PostRepository.GetPostsByUserId(userId);
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 10) pageSize = 10;

            var querry = _context.Posts.Include(p => p.PostsImages).Include(p => p.User).
                        Include(p => p.Comments).ThenInclude(c => c.CommentLikes).AsNoTracking().Where(p => p.UserId == userId);

            var totalCount = querry.Count();

            var postEntities = await querry.OrderByDescending(x => x.PostedAt).Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            var posts = postEntities.Select(p => new PostClientDto
            {
                UserId = p.UserId,
                Content = p.Content,
                Username = p.User.Username,
                PostId = p.Id,
                PostImagesUrl = p.PostsImages.Select(x => x.ImgUrl.ToFullImageUrl()).ToList(),
                ProfileImgUrl = p.User.ProfileImageUrl.ToFullImageUrl(),
                PostedAt = p.PostedAt,

                Likes = _context.Likes.Include(x => x.User).AsNoTracking().Where(like => like.PostId == p.Id).Select((l => new UserBasicInfo
                {
                    UserId = l.UserId,
                    ProfileImageUrl = l.User.ProfileImageUrl == null ? null : l.User.ProfileImageUrl.ToFullImageUrl(),
                    Username = l.User.Username,
                })).ToList(),
                LikeCount = _context.Likes.AsNoTracking().Where(l => l.PostId == p.Id).Count(),
                Comments = _context.Comments.Where(x => x.PostId == p.Id).Select(c => new CommentClientDto
                {
                    UserId = c.UserId,
                    CommentedAt = c.CommentedAt,
                    CommentId = c.Id,
                    Content = c.Content,
                    UserProfileImageUrl = c.User.ProfileImageUrl.ToFullImageUrl(),
                    CommentImageUrl = c.CommentImageUrl.ToFullImageUrl(),
                    Username = c.User.Username,
                    CommentLikes = c.CommentLikes.Where(x => x.CommentId == c.Id).Select(like => new UserBasicInfo
                    {
                        ProfileImageUrl = like.User.ProfileImageUrl.ToFullImageUrl(),
                        Username = like.User.Username,
                        UserId = like.UserId,
                    }).ToList(),
                }).OrderByDescending(x => x.CommentedAt).ToList(),


            }).ToList();

            var PageResult = new PageResult<PostClientDto>
            {
                Items = posts,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
            return PageResult;
        }

        public Task<Post> GetPostById(Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task<string> LikePost(Guid UserId, Guid PostId)
        {
            throw new NotImplementedException();
        }
    }
}

using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Extensions;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Friendshub.Infrastructure.Implementations
{
    public class PostRepository : IPostRepository
    {
        private readonly FriendshubDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly INotificationRepository _notificationRepository;
        public PostRepository(FriendshubDbContext context, IWebHostEnvironment webHostEnvironment, INotificationRepository notificationRepository)
        {
            _context = context;
            _env = webHostEnvironment;
            _notificationRepository = notificationRepository;
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

                        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/posts/images");

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

            _context.Posts.Add(newPost);

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


        public async Task<CommentClientDto> CommentPost(Guid userId, Post post, AddCommentDto comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Content) && (comment.Image == null || comment.Image.Length == 0))
                throw new ApplicationException("You have to add comment or a picture.");

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x =>x.Id == userId);

            var newComment = new Comment
            {
                Id = Guid.NewGuid(),
                PostId = post.Id,
                CommentedAt = DateTime.UtcNow,
                Content = comment.Content,
                UserId = userId,
            };

            if (comment.Image != null && comment.Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(comment.Image.FileName);
                var uploadFolder = Path.Combine("wwwroot", "uploads", "comments");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await comment.Image.CopyToAsync(stream);

                newComment.CommentImageUrl = "/uploads/comments/" + fileName;
            }
            var commentDto = new CommentClientDto()
            {
                UserId = userId,
                Username = user.Username,
                UserProfileImageUrl = string.IsNullOrWhiteSpace(user.ProfileImageUrl) ? null : user.ProfileImageUrl,
                CommentedAt = newComment.CommentedAt,
                Content = newComment.Content,
                CommentId = newComment.Id,
                CommentImageUrl = newComment.CommentImageUrl.ToFullImageUrl(),
            };
            await _context.Comments.AddAsync(newComment);
            return commentDto;
        }


        public async Task<bool> DeletePost(Guid postId)
        {
            var post = await _context.Posts.Include(post => post.PostsImages).Where(x => x.Id == postId).SingleOrDefaultAsync();
            if (post == null)
                return false;

            if (post.PostsImages.Count() > 0)
            {
                foreach (var image in post.PostsImages)
                {
                    if (!string.IsNullOrWhiteSpace(image.ImgUrl))
                    {
                        var physicalPath = Path.Combine(_env.WebRootPath, image.ImgUrl.Replace('/', Path.DirectorySeparatorChar));
                        if (File.Exists(physicalPath))
                        {
                            try
                            {
                                File.Delete(physicalPath);
                            }
                            catch (Exception exc)
                            {
                                Console.WriteLine(exc.Message);
                            }
                        }
                    }
                }
            }
            _context.Posts.Remove(post);
            return true;
        }
        public async Task<Comment> GetCommentById(Guid commentId)
        {
            return await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c =>  c.Id == commentId);
        }

        public async Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId, int pageNumber = 1)
        {
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 10) pageSize = 10;

            var followingUsersIds = await _context.Follows.AsNoTracking().Where(x => x.FollowerId == userId)
                                   .Select(x => x.FolloweeId).ToListAsync();

            var querry = _context.Posts.Include(p => p.PostsImages).Include(p => p.User).
                        Include(p => p.Comments).ThenInclude(c => c.CommentLikes).AsNoTracking().Where(p => p.UserId == userId || followingUsersIds.Contains(p.UserId));
            
            var totalCount = querry.Count();

            var postEntities = await querry.OrderByDescending(x => x.PostedAt).Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).AsNoTracking().ToListAsync();

                var posts = postEntities.Select(p => new PostClientDto
                {
                    UserId = p.UserId,
                    Content = p.Content,
                    Username = p.User.Username,
                    PostId = p.Id,
                    PostImagesUrl = p.PostsImages.Select(x => x.ImgUrl.ToFullImageUrl()).ToList(),
                    ProfileImgUrl = p.User.ProfileImageUrl.ToFullImageUrl(),
                    PostedAt = p.PostedAt,
                    
                    Likes = _context.Likes.Include("User").Where(like => like.PostId == p.Id).Select((l => new UserBasicInfo
                    {
                        UserId = l.UserId,
                        ProfileImageUrl = l.User.ProfileImageUrl == null ? null : l.User.ProfileImageUrl.ToFullImageUrl(),
                        Username = l.User.Username,
                    })).ToList(),
                    LikeCount = _context.Likes.AsNoTracking().Where(x => x.PostId == p.Id).Count(),
                    Comments = _context.Comments.AsNoTracking().Where(x => x.PostId == p.Id).Select(c => new CommentClientDto
                    {
                        UserId = c.UserId,
                        CommentedAt = c.CommentedAt,
                        CommentId = c.Id,
                        Content = c.Content,
                        UserProfileImageUrl = c.User.ProfileImageUrl.ToFullImageUrl(),
                        CommentImageUrl = c.CommentImageUrl.ToFullImageUrl(),
                        Username =c.User.Username,
                        CommentLikes = c.CommentLikes.Where(x => x.CommentId == c.Id).Select(like => new UserBasicInfo
                        {
                            ProfileImageUrl = like.User.ProfileImageUrl.ToFullImageUrl(),
                            Username = like.User.Username,
                            UserId = like.UserId,
                        }).ToList(),
                    }).OrderByDescending(x =>x.CommentedAt).ToList(),
                    
                    
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
        public async Task<PageResult<PostClientDto>> GetMyPosts(Guid userId, int pageNumber = 1)
        {
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

        public async Task<Post> GetPostById(Guid postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);
            return post;
        }

        public async Task<LikeCommentResponseDto> LikePostComment(Guid commentId, Guid userId)
        {
            var response = new LikeCommentResponseDto();
            var existingLike = await _context.CommentsLikes.FirstOrDefaultAsync((x => x.UserId == userId && x.CommentId == commentId));
            if(existingLike != null)
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
        public async Task<string> LikePost(Guid userId, Guid postId)
        {
            string message = string.Empty;
            var post = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == postId);

            var like = await _context.Likes.FirstOrDefaultAsync(x => x.UserId == userId && postId == x.PostId);
            if (like == null)
            {
                var newlike = new PostLike()
                {
                    UserId = userId,
                    PostId = postId,
                    LikedAt = DateTime.UtcNow
                };
                await _notificationRepository.CreateNotification(userId, post.UserId, NotificationType.Like, postId);
                _context.Likes.Add(newlike);

                message = "Post liked.";
            }
            else
            {
            message = "Post disliked.";
                _context.Likes.Remove(like);
            }
            return message;
        }

        public async Task<bool> DeleteComment(Guid commentId, Guid userId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == commentId);
            if (comment == null)
                return false;
            _context.Comments.Remove(comment);
            return true;
        }
    }
}

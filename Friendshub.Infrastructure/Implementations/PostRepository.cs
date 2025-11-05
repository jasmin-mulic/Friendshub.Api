using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Extensions;
using Friendshub.Application.Interfaces.Repositories;
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

        public async Task AddPostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
        }


        public async Task<CommentClientDto> AddCommentToPost(Guid userId, Post post, AddCommentDto comment)
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

            if (comment.Image != null && comment.Image.Length == 1)
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


        public void DeletePost(Post post)
        {
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
        }
        public async Task<Comment> GetPostCommentById(Guid commentId)
        {
            return await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c =>  c.Id == commentId);
        }

        public async Task<PageResult<PostClientDto>> GetFeedPostsPaged(Guid userId,List<Guid> followingUsersIds, int pageNumber = 1)
        {
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 10) pageSize = 10;


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
                    
                    Likes = _context.PostLikes.Include("User").Where(like => like.PostId == p.Id).Select((l => new UserBasicInfo
                    {
                        UserId = l.UserId,
                        ProfileImageUrl = l.User.ProfileImageUrl == null ? null : l.User.ProfileImageUrl.ToFullImageUrl(),
                        Username = l.User.Username,
                    })).ToList(),
                    LikeCount = _context.PostLikes.AsNoTracking().Where(x => x.PostId == p.Id).Count(),
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

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);
            return post;
        }

        public async Task<LikeCommentResponseDto> LikePostComment(Guid commentId, Guid userId)
        {
            var response = new LikeCommentResponseDto();
            var existingLike = await _context.CommentLikes.FirstOrDefaultAsync((x => x.UserId == userId && x.CommentId == commentId));
            if(existingLike != null)
            {
                response.CommentId = commentId;
                response.Message = "disliked";
                response.User.UserId = userId;
                _context.CommentLikes.Remove(existingLike);
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
            await _context.CommentLikes.AddAsync(newLike);
            return response;
        }
        public async Task<string> LikePost(Guid userId, Guid postId)
        {
            string message = string.Empty;
            var post = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == postId);

            var like = await _context.PostLikes.FirstOrDefaultAsync(x => x.UserId == userId && postId == x.PostId);
            if (like == null)
            {
                var newlike = new PostLike()
                {
                    UserId = userId,
                    PostId = postId,
                    LikedAt = DateTime.UtcNow
                };
                await _notificationRepository.CreateNotification(userId, post.UserId, NotificationType.Like, postId);
                _context.PostLikes.Add(newlike);

                message = "Post liked.";
            }
            else
            {
            message = "Post disliked.";
                _context.PostLikes.Remove(like);
            }
            return message;
        }

        public async Task<PageResult<PostClientDto>> GetUserPostsByIdPaged(Guid userId, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var baseQuery = _context.Posts
                .AsNoTracking()
                .Where(p => p.UserId == userId);

            var totalCount = await baseQuery.CountAsync();

            var posts = await baseQuery
                .OrderByDescending(p => p.PostedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostClientDto
                {
                    PostId = p.Id,
                    UserId = p.UserId,
                    Username = p.User.Username,
                    Content = p.Content,
                    ProfileImgUrl = p.User.ProfileImageUrl.ToFullImageUrl(),
                    PostedAt = p.PostedAt,

                    PostImagesUrl = p.PostsImages
                        .Select(i => i.ImgUrl.ToFullImageUrl())
                        .ToList(),

                    Likes = p.Likes.Select(l => new UserBasicInfo
                    {
                        UserId = l.UserId,
                        Username = l.User.Username,
                        ProfileImageUrl = l.User.ProfileImageUrl.ToFullImageUrl()
                    }).ToList(),

                    LikeCount = p.Likes.Count(),

                    Comments = p.Comments
                        .OrderByDescending(c => c.CommentedAt)
                        .Select(c => new CommentClientDto
                        {
                            CommentId = c.Id,
                            UserId = c.UserId,
                            Username = c.User.Username,
                            Content = c.Content,
                            CommentedAt = c.CommentedAt,
                            CommentImageUrl = c.CommentImageUrl.ToFullImageUrl(),
                            UserProfileImageUrl = c.User.ProfileImageUrl.ToFullImageUrl(),
                            CommentLikes = c.CommentLikes.Select(like => new UserBasicInfo
                            {
                                UserId = like.UserId,
                                Username = like.User.Username,
                                ProfileImageUrl = like.User.ProfileImageUrl.ToFullImageUrl()
                            }).ToList()
                        }).ToList()
                })
                .ToListAsync();

            return new PageResult<PostClientDto>
            {
                Items = posts,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<List<PostLike>> GetPostLikes(Guid PostId)
        {
            return await _context.PostLikes.AsNoTracking().Where(x => x.PostId == PostId).ToListAsync();
        }
        public async Task<List<PostClientDto>> GetFeedPosts(Guid userId, int pageNumber, int pageSize)
        {
            var query = _context.Posts
             .AsNoTracking();

            var totalCount = await query.CountAsync();

            var posts = await query
                .OrderByDescending(x => x.PostedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostClientDto
                {
                    UserId = p.UserId,
                    Content = p.Content,
                    Username = p.User.Username,
                    PostId = p.Id,
                    PostImagesUrl = p.PostsImages.Select(x => x.ImgUrl.ToFullImageUrl()).ToList(),
                    ProfileImgUrl = p.User.ProfileImageUrl.ToFullImageUrl(),
                    PostedAt = p.PostedAt,

                    Likes = p.Likes.Select(l => new UserBasicInfo
                    {
                        UserId = l.UserId,
                        ProfileImageUrl = l.User.ProfileImageUrl.ToFullImageUrl(),
                        Username = l.User.Username,
                    }).ToList(),
                    LikeCount = p.Likes.Count(),

                    Comments = p.Comments.Select(c => new CommentClientDto
                    {
                        UserId = c.UserId,
                        CommentedAt = c.CommentedAt,
                        CommentId = c.Id,
                        Content = c.Content,
                        UserProfileImageUrl = c.User.ProfileImageUrl.ToFullImageUrl(),
                        CommentImageUrl = c.CommentImageUrl.ToFullImageUrl(),
                        Username = c.User.Username,

                        CommentLikes = c.CommentLikes.Select(like => new UserBasicInfo
                        {
                            ProfileImageUrl = like.User.ProfileImageUrl.ToFullImageUrl(),
                            Username = like.User.Username,
                            UserId = like.UserId,

                        }).ToList(),
                    }).OrderByDescending(x => x.CommentedAt).ToList()
                }).ToListAsync();

            return posts;
        }

        public async Task<int> GetUserPostCount(Guid userId)
        {
            return await _context.Posts
                   .Where(x =>  x.UserId == userId)
                   .AsNoTracking()
                   .CountAsync();
        }
    }
}

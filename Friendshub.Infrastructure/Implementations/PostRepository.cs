using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Extensions;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Friendshub.Infrastructure.Implementations
{
    public class PostRepository : IPostRepository
    {
        private readonly FriendshubDbContext _context;
        public PostRepository(FriendshubDbContext context)
        {
            _context = context;
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

                        // Folder unutar wwwroot
                        var uploadsFolder = Path.Combine("wwwroot", "uploads", "posts");

                        // Kreiranje foldera ako ne postoji
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        // Putanja za spremanje na disk
                        var physicalPath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(physicalPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // U bazi čuvamo relativnu putanju
                        var relativePath = Path.Combine("uploads", "posts", fileName).Replace("\\", "/");

                        newPost.PostsImages.Add(new PostImage
                        {
                            ImgUrl = relativePath,
                            Post = newPost
                        });
                    }
                }
            }

            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync(); // ne zaboravi sačuvati

            var postDto = new PostClientDto
            {
                Content = newPost.Content,
                Username = newPost.User.DisplayUsername,
                PostId = newPost.Id,
                PostImagesUrl = newPost.PostsImages.Select(x => x.ImgUrl.ToFullImageUrl()).ToList(),
                PostedAt = newPost.PostedAt,
            };

            return postDto;
        }


        public async Task<CommentClientDto> CommentPost(Guid userId, Post post, AddCommentDto comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Content) && (comment.Image == null || comment.Image.Length == 0))
                throw new ApplicationException("You have to add comment or a picture.");

            var user = await _context.Users.FirstOrDefaultAsync(x =>x.Id == userId);

            var newComment = new Comment
            {
                Id = Guid.NewGuid(),
                PostId = post.Id,
                CommentedAt = DateTime.UtcNow,
                Content = comment.Content,
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
                Username = user.Username,
                UserProfileImageUrl = newComment.CommentImageUrl,
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
            var comments = _context.Comments.Where(x => x.PostId == post.Id).ToList();
            if(comments.Count > 0) 
                _context.Comments.RemoveRange(comments);
            _context.Posts.Remove(post);    
        }

        public async Task<Comment> GetCommentById(Guid commentId)
        {
            return await _context.Comments.FirstOrDefaultAsync(c =>  c.Id == commentId);
        }

        public async Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId, int pageNumber = 1)
        {
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 10) pageSize = 10;

            var followingUsersIds = await _context.Follows.Where(x => x.FollowerId == userId)
                                   .Select(x => x.FolloweeId).ToListAsync();

            var querry = _context.Posts.Include(p => p.PostsImages).Include(p => p.User).
                        Include(p => p.Comments).ThenInclude(c => c.CommentLikes).Where(p => p.UserId == userId || followingUsersIds.Contains(p.UserId));
            
            var totalCount = querry.Count();

            var postEntities = await querry.OrderByDescending(x => x.PostedAt).Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync();

                var posts = postEntities.Select(p => new PostClientDto
                {
                    UserId = p.UserId,
                    Content = p.Content,
                    Username = p.User.DisplayUsername,
                    PostId = p.Id,
                    PostImagesUrl = p.PostsImages.Select(x => x.ImgUrl.ToFullImageUrl()).ToList(),
                    ProfileImgUrl = p.User.ProfileImgUrl.ToFullImageUrl(),
                    PostedAt = p.PostedAt,
                    
                    Likes = GetLikes(p.Id),
                    Comments = _context.Comments.Where(x => x.PostId == p.Id).Select(c => new CommentClientDto
                    {
                        CommentedAt = c.CommentedAt,
                        CommentId = c.Id,
                        Content = c.Content,
                        UserProfileImageUrl = p.User.ProfileImgUrl,
                        CommentImageUrl = c.CommentImageUrl.ToFullImageUrl(),
                        Username = p.User.DisplayUsername,
                        CommentLikes = c.CommentLikes.Where(x => x.UserId == userId).Select(like => new UserBasicInfo
                        {
                            ProfileImageUrl = like.User.ProfileImgUrl.ToFullImageUrl(),
                            Username = like.User.DisplayUsername,
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

        public  PostLikes GetLikes(Guid postId)
        {
            var likes = _context.Likes.Include(x => x.User).Where(l => l.PostId == postId).ToList();
            var postLikes = new PostLikes()
            {
                Count = likes.Count,
                Users = likes.Select(x => new UserBasicInfo
                {
                    UserId = x.User.Id,
                    ProfileImageUrl = x.User.ProfileImgUrl,
                    Username = x.User.DisplayUsername
                }).ToList(),
            };
            return postLikes;
        }

        public async Task<PageResult<PostClientDto>> GetMyPosts(Guid userId, int pageNumber = 1)
        {
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 10) pageSize = 10;

            var querry = _context.Posts.Include(p => p.PostsImages).Include(p => p.User).Include(p => p.Comments).Where(x => x.UserId == userId).OrderByDescending(x => x.PostedAt);
            var totalCount = querry.Count();

            var postsEntities = await querry.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

                var posts = postsEntities.Select(p => new PostClientDto
                {
                    PostId = p.Id,
                    Content = p.Content,
                    UserId = p.UserId,
                    PostedAt = p.PostedAt,
                    Username = p.User.DisplayUsername,
                    PostImagesUrl = p.PostsImages.Select(postImg => postImg.ImgUrl.ToFullImageUrl()).ToList(),
                    Likes = GetLikes(p.Id),
                    Comments = p.Comments.Select(c => new CommentClientDto
                    {
                        CommentedAt = c.CommentedAt,
                        Content = c.Content,
                        CommentId = c.Id,
                        Username = c.Post.User.DisplayUsername,
                        CommentImageUrl = c.CommentImageUrl.ToFullImageUrl(),
                        UserProfileImageUrl = c.Post.User.ProfileImgUrl,
                    }).OrderByDescending(x => x.CommentedAt).ToList()
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

        public async Task<string> LikeComment(Guid userId, Guid CommentId)
        {
            string responseMessage = string.Empty;
            var like = await _context.CommentsLikes.FirstOrDefaultAsync(x => x.UserId == userId);
            if (like != null)
            {
                responseMessage = "Comment disliked";
            _context.CommentsLikes.Remove(like);
            return responseMessage;

            }


            var newLike = new CommentLike
            {
                UserId = userId,
                CommentId = CommentId,
                LikedAt = DateTime.Now,
            };
            responseMessage = "Comment liked";
            await _context.CommentsLikes.AddAsync(newLike);
            return responseMessage;
        }
        public async Task<string> LikePost(Guid userId, Guid postId)
        {
            string message = string.Empty;
            var like = await _context.Likes.FirstOrDefaultAsync(x => x.UserId == userId && postId == x.PostId);
            if (like == null)
            {
                var newlike = new PostLike()
                {
                    UserId = userId,
                    PostId = postId,
                    LikedAt = DateTime.UtcNow
                };
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


    }
}

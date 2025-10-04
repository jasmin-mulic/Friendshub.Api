using Friendshub.Application.DTO;
using Friendshub.Application.DTO.Post;
using Friendshub.Application.DTO.User;
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

        public async Task<Post> AddPost(AddPostDto request, Guid UserId)
        {
            if (string.IsNullOrWhiteSpace(request.Content) && (request.ImagePaths == null || request.ImagePaths.Count == 0))
                return null;

            var newPost = new Post
            {
                Content = string.IsNullOrWhiteSpace(request.Content) ? null : request.Content,
                UserId = UserId
            };

            if (request.ImagePaths != null && request.ImagePaths.Count > 0)
            {
                foreach (var file in request.ImagePaths)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine("wwwroot/uploads/post/", fileName);
                        var uploadsFolder = Path.Combine("wwwroot", "uploads", "post");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        newPost.PostsImages.Add(new PostImage
                        {
                            ImgUrl = "https://localhost:44326/uploads/post/" + fileName,
                            Post = newPost
                        });
                    }
                }
            }
            _context.Posts.Add(newPost);
            return newPost;
        }

        public void DeletePost(Post post)
        {
            _context.Posts.Remove(post);    
        }
        public async Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId)
        {
            int pageNumber = 1;
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 10) pageSize = 10;

            var followingUsersIds = await _context.Follows.Where(x => x.FollowerId == userId)
                                   .Select(x => x.FolloweeId).ToListAsync();

            var querry = _context.Posts.Include(p => p.PostsImages).Include(p => p.User)
                        .Where(p => p.UserId == userId || followingUsersIds.Contains(p.UserId));
            
            var totalCount = querry.Count();

            var postEntities = await querry.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToListAsync();

                var posts = postEntities.Select(p => new PostClientDto
                {
                    Content = p.Content,
                    Username = p.User.DisplayUsername,
                    PostId = p.Id,
                    PostImagesUrl = p.PostsImages.Select(x => x.ImgUrl).ToList(),
                    PostedAt = p.PostedAt,
                    Likes = GetLikes(p.Id)

                }).OrderByDescending(x => x.PostedAt).ToList();

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

        public async Task<PageResult<PostClientDto>> GetMyPosts(Guid userId)
        {
            int pageNumber = 1;
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 10) pageSize = 10;
            if (pageSize > 10) pageSize = 10;

            var querry = _context.Posts.Include(p => p.PostsImages).Include(p => p.User).Where(x => x.UserId == userId).OrderByDescending(x => x.PostedAt);
            var totalCount = querry.Count();

            var postsEntities = await querry.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

                var posts = postsEntities.Select(p => new PostClientDto
                {
                    PostId = p.Id,
                    Content = p.Content,
                    PostedAt = p.PostedAt,
                    Username = p.User.DisplayUsername,
                    PostImagesUrl = p.PostsImages.Select(postimg => postimg.ImgUrl).ToList(),
                    Likes = GetLikes(p.Id)
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

        public async Task<string> LikePost(Guid userId, Guid postId)
        {
            string message = string.Empty;
            var like = await _context.Likes.FirstOrDefaultAsync(x => x.UserId == userId && postId == x.PostId);
            if (like == null)
            {
                var newlike = new Like()
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

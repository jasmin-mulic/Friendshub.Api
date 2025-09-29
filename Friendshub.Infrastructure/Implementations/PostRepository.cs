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
            if (string.IsNullOrWhiteSpace(request.Content) && (request.PostImagesUrls == null || request.PostImagesUrls.Count == 0))
                return null;

            var newPost = new Post
            {
                Content = string.IsNullOrWhiteSpace(request.Content) ? null : request.Content,
                UserId = UserId
            };

            if (request.PostImagesUrls != null && request.PostImagesUrls.Count > 0)
            {
                foreach (var file in request.PostImagesUrls)
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

            var posts = await querry.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostClientDto
                {
                    Content = p.Content,
                    Username = p.User.DisplayUsername,
                    PostId = p.Id,
                    PostImagesUrl = p.PostsImages.Select(x => x.ImgUrl).ToList(),
                    PostedAt = p.PostedAt,
                    LikeCounter = p.LikeCounter,
                }).OrderByDescending(x => x.PostedAt).ToListAsync();

            var PageResult = new PageResult<PostClientDto>
            {
                Items = posts,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
            return PageResult;
        }

        public async Task<PageResult<PostClientDto>> GetMyPosts(Guid userId)
        {
            int pageNumber = 1;
            int pageSize = 10;
            if(pageNumber < 1) pageNumber = 1;
            if(pageSize < 10) pageSize = 10;
            if(pageSize > 10) pageSize = 10;

            var querry = _context.Posts.Include(p => p.PostsImages).Include(p => p.User).Where(x => x.UserId == userId).OrderByDescending(x => x.PostedAt);
            var totalCount = querry.Count();

            var posts = await querry.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostClientDto
                {
                    PostId = p.Id,
                    Content = p.Content,
                    PostedAt = p.PostedAt,
                    Username = p.User.DisplayUsername,
                    PostImagesUrl = p.PostsImages.Select(postimg => postimg.ImgUrl).ToList(),
                    LikeCounter = p.LikeCounter,
                }).ToListAsync();
            var PageResult = new PageResult<PostClientDto>
            {
                Items = posts,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
            return PageResult;
             }
    }
}

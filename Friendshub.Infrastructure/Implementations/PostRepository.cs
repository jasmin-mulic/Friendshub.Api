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
        public async Task<List<Post>> GetFeedPostsPaged(Guid userId,List<Guid> followingUsersIds, int pageNumber = 1)
        {
            int pageSize = 10;
            if (pageNumber < 1) pageNumber = 1;

            var query = _context.Posts
                .Include(p => p.PostsImages)
                .Include(p => p.User)

                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)

                .Include(p => p.Comments)
                    .ThenInclude(c => c.CommentLikes)
                        .ThenInclude(cl => cl.User)
                .Include(p => p.Likes)
                .ThenInclude(pl => pl.User)
                .Where(p => p.UserId == userId || followingUsersIds.Contains(p.UserId))
                .AsNoTracking();

            return await query
                .OrderByDescending(x => x.PostedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == postId);
            return post;
        }
        public async Task<List<Post>> GetUserPostsByIdPaged(Guid userId, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;

            var query = _context.Posts
                .Include(p => p.PostsImages)
                .Include(p => p.User)
                .Include(p => p.Comments).ThenInclude(c => c.CommentLikes)
                .Where(p => p.UserId == userId)
                .AsNoTracking();

            return await query
                .OrderByDescending(x => x.PostedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<PostLike>> GetPostLikes(Guid PostId)
        {
            return await _context.PostLikes.AsNoTracking().Where(x => x.PostId == PostId).ToListAsync();
        }

        public async Task<int> UserPostTotalCount(Guid userId)
        {
            return await _context.Posts
                   .Where(x =>  x.UserId == userId)
                   .AsNoTracking()
                   .CountAsync();
        }

        public async Task<int> FeedPostsTotalCount(Guid userId, List<Guid> followingUsersIds)
        {
            return await _context.Posts.AsNoTracking().Where(p => p.UserId == userId || followingUsersIds.Contains(p.UserId)).CountAsync();
        }
    }
}

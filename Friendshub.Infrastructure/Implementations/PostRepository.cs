using Friendshub.Application.DTO.Post;
using Friendshub.Application.DTO.User;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Infrastructure.Implementations
{
    public class PostRepository : IPostRepository
    {
        private readonly FriendshubDbContext _context;
        public PostRepository(FriendshubDbContext context)
        {
            _context = context;
        }
        public Task<Post> AddPost(AddPostDto request, Guid UserId)
        {
            throw new NotImplementedException();
        }

        public Task<PostClientDto> GetFeedPosts(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PostClientDto>> GetMyPosts(Guid userId)
        {
            var userPosts = await _context.Posts.Include(p => p.PostsImages).Where(x => x.UserId == userId)
                                                .Select(x => new PostClientDto
                                                {
                                                    Content = x.Content,
                                                    CreatedAt = x.PostedAt,
                                                    PostImagesUrl = x.PostsImages.Select(x => x.ImgUrl).ToList(),
                                                    PostId = x.Id
                                                }).ToListAsync();

            return userPosts;
        }
    }
}

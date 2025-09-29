using Friendshub.Application.DTO.Post;
using Friendshub.Application.DTO.User;
using Friendshub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Repositories
{
    public interface IPostRepository
    {
        Task<Post> AddPost(AddPostDto request, Guid UserId);
        Task<List<PostClientDto>> GetMyPosts(Guid userId);
        Task<PostClientDto> GetFeedPosts(Guid userId);
    }
}

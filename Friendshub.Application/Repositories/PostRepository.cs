using Friendshub.Application.DTO;
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
        Task<PostClientDto> AddPost(AddPostDto request, Guid UserId);
        Task<PageResult<PostClientDto>>GetMyPosts(Guid userId, int page);
        Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId, int page);
        void DeletePost(Post post);
        Task<Post> GetPostById(Guid postId);
        Task<string> LikePost(Guid UserId, Guid PostId);
        PostLikes GetLikes(Guid postId);
    }
}

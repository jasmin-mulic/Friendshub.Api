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
        Task<Post> AddPost(AddPostDto request, Guid UserId);
        Task<PageResult<PostClientDto>>GetMyPosts(Guid userId);
        Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId);
        void DeletePost(Post post);
        Task<Post> GetPostById(Guid postId);
        void LikePost(Guid UserId, Guid PostId);
        PostLikes GetLikes(Guid postId);
    }
}

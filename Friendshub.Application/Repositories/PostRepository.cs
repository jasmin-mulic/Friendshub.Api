using Friendshub.Application.DTO;
using Friendshub.Application.DTO.DtoPost;
using Friendshub.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Repositories
{
    public interface IPostRepository
    {
        Task<PostClientDto> AddPost(AddPostDto request, User user);
        Task<PageResult<PostClientDto>>GetMyPosts(Guid userId, int page);
        Task<PageResult<PostClientDto>> GetFeedPosts(Guid userId, int page);
        void DeletePost(Post post);
        Task<Post> GetPostById(Guid postId);
        Task<string> LikePost(Guid UserId, Guid PostId);
        PostLikes GetLikes(Guid postId);
        Task<Comment> CommentPost (Guid userId, Post post, string content, IFormFile image);
    }
}

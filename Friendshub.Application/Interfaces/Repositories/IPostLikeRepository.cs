using Friendshub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Interfaces.Repositories
{
    public interface IPostLikeRepository
    {
        void RemoveLike(PostLike like);
        Task AddLike(PostLike like);
        Task<List<PostLike>> GetPostLikes(Guid postId);
        Task<PostLike> GetPostLikeForUser(Guid postId, Guid userId);
    }
}

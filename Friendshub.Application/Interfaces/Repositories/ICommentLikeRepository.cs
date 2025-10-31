using Friendshub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Interfaces.Repositories
{
    public interface ICommentLikeRepository
    {
        Task<CommentLike> GetUserLikeAsync(Guid userId, Guid commentId);
    }
}

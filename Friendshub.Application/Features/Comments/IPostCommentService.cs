using Friendshub.Application.Features.Posts.DTO;
using Friendshub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Features.Comments
{
    public interface IPostCommentService
    {
        Task<CommentClientDto> AddCommentPost(Guid userId, Post post, AddCommentDto comment);


    }
}

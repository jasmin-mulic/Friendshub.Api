using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
using Friendshub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Interfaces.Services
{
    public interface IPostCommentService
    {
        Task<CommentClientDto> AddCommentPost(Guid userId, Post post, AddCommentDto comment);


    }
}

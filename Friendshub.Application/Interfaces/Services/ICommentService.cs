using Friendshub.Application.DTO.DtoPost;
using Friendshub.Application.DTO.PostDto;
using Friendshub.Domain.Models;

namespace Friendshub.Application.Interfaces.Services
{
    public interface ICommentService
    {
        Task<CommentClientDto> AddComment(Guid userId, Post post, AddCommentDto commentRequest);
        Task RemoveComment(Guid commentId);
    }
}

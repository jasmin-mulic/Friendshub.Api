using Friendshub.Application.Interfaces;
using Friendshub.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
         IAuthRepository AuthRepository { get;}
         ITokenRepository TokenRepository { get;}
         IUserRepository UserRepository { get;}
         IPostRepository PostRepository { get;}
         IUserRoleRepository UserRoleRepository { get;}
         INotificationRepository NotificationRepository { get;}
         IFollowRepository FollowRepository { get;}
         IPostLikeRepository PostLikeRepository { get;}
         ICommentRepository CommentRepository { get;}
         ICommentLikeRepository CommentLikeRepository { get;}
         Task<bool> ApplyChangesAsync();
    }
}

using Friendshub.Application.Interfaces;
using Friendshub.Application.Interfaces.Repositories;
namespace Friendshub.Application.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
         ITokenRepository TokenRepository { get;}
         IUserRepository UserRepository { get;}
         IPostRepository PostRepository { get;}
         IUserRoleRepository UserRoleRepository { get;}
         INotificationRepository NotificationRepository { get;}
         IFollowRepository FollowRepository { get;}
         IPostLikeRepository PostLikeRepository { get;}
         ICommentRepository CommentRepository { get;}
         ICommentLikeRepository CommentLikeRepository { get;}
         IFollowRequestRepository FollowRequestRepository { get;}
         Task<bool> ApplyChangesAsync();
    }
}

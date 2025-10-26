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
         INotificationRepository NotificationRepository { get;}
         Task<bool> ApplyChanges();
    }
}

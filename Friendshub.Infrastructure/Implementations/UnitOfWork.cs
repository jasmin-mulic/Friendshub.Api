using Friendshub.Application.Interfaces;
using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Application.Repositories;
using Friendshub.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
namespace Friendshub.Infrastructure.Implementations
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly FriendshubDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private bool _disposed = false;
        public ITokenRepository TokenRepository { get; }
        public IUserRepository UserRepository { get; }
        public ICommentRepository CommentRepository { get; }
        public IFollowRequestRepository FollowRequestRepository { get; }
        public IPostRepository PostRepository { get; }
        public INotificationRepository NotificationRepository { get; }
        public IUserRoleRepository UserRoleRepository { get; }
        public IFollowRepository FollowRepository { get; }
        public IPostLikeRepository PostLikeRepository { get; }
        public ICommentLikeRepository CommentLikeRepository { get; }
        public UnitOfWork(FriendshubDbContext context,
                         IConfiguration configuration,
                         IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            TokenRepository = new TokenRepository(_context, _configuration);
            UserRepository = new UserRepository(_context, _webHostEnvironment);
            NotificationRepository = new NotificationRepository(_context);
            PostRepository = new PostRepository(_context, webHostEnvironment, NotificationRepository);
            UserRoleRepository = new UserRoleRepository(_context);
            FollowRepository = new FollowRepository(_context);
            PostLikeRepository = new PostLikeRepository(_context);
            CommentRepository = new CommentRepository(_context);
            CommentLikeRepository = new CommentLikeRepository(_context);
            FollowRequestRepository = new FollowRequestRepository(_context);
        }

        public async Task<bool> ApplyChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;

            }
            catch (Exception exc)
            {

                throw new ApplicationException("Error saving changes to database", exc);
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                {
                    _context?.Dispose();
                }
            _disposed = true;
        }
    }
}

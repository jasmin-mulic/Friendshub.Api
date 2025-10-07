using Friendshub.Application.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
namespace Friendshub.Infrastructure.Data.Implementations
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly FriendshubDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private bool _disposed = false;
        public UnitOfWork(FriendshubDbContext context,
                         IConfiguration configuration, 
                         IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            TokenRepository = new TokenRepository(_context, _configuration);
            AuthRepository = new AuthRepository(_context, TokenRepository);
            UserRepository = new UserRepository(_context, _webHostEnvironment);
            PostRepository = new PostRepository(_context);
            
        }
        public IAuthRepository AuthRepository { get; }
        public ITokenRepository TokenRepository { get; }
        public IUserRepository UserRepository { get; }
        public IPostRepository PostRepository { get; }


        public async Task<bool> ApplyChanges()
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
            if(!_disposed)
                if(disposing)
                {
                    _context?.Dispose();
                }
            _disposed = true;
        }
    }
}

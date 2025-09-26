using Friendshub.Application.Repositories;
using Friendshub.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Infrastructure.Implementations
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly FriendshubDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private bool _disposed = false;
        public UnitOfWork(FriendshubDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            TokenRepository = new TokenRepository(_context, _configuration);
            AuthRepository = new AuthRepository(_context, TokenRepository);
            _webHostEnvironment = webHostEnvironment;
            UserRepository = new UserRepository(_context, _webHostEnvironment);
        }
        public IAuthRepository AuthRepository { get; }
        public ITokenRepository TokenRepository { get; }
        public IUserRepository UserRepository { get; }


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

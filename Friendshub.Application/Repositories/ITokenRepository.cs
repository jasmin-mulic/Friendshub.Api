using Friendshub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Repositories
{
    public interface ITokenRepository
    {
        Task<string> CreateAccessToken(User user);
        string CreateRefreshToken();
        Task<RefreshToken> GetUserRefreshToken(User user);
        Task<RefreshToken> AddRefreshToken(Guid userId);
        Task<RefreshToken> GetRefreshTokenByValue(string value);
        void DeleteRefreshToken(Guid UserId);
        
    }
}

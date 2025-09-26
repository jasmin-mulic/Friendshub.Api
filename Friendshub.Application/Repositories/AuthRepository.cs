using Friendshub.Application.DTO.Auth;
using Friendshub.Application.Results;
using Friendshub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Repositories
{
    public interface IAuthRepository
    {
        Task<LoginResult> LoginAsync(LoginUserDto request);
        Task<RegisterResult> RegisterAsync(RegisterUserDto request);
    }
}

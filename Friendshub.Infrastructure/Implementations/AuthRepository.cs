using Friendshub.Application.DTO;
using Friendshub.Application.DTO.Auth;
using Friendshub.Application.Repositories;
using Friendshub.Application.Results;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Friendshub.Infrastructure.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly FriendshubDbContext _context;
        private readonly ITokenRepository _tokenRepository;
        public AuthRepository(FriendshubDbContext context, ITokenRepository tokenRepository)
        {
            _context = context;
            _tokenRepository = tokenRepository;
        }
        public async Task<LoginResult> LoginAsync(LoginUserDto request)
        {
            var result = new LoginResult();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == request.UsernameOrEmail.ToLower() ||
                                                                     x.EmailAddress == request.UsernameOrEmail.ToLower());
   
            if (user == null || BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.PasswordHash) == false)
            {
                result.Success = false;
                return result;
            }
            result.Success = true;
            result.AccessToken = await _tokenRepository.CreateAccessToken(user);
            result.User = user;
            return result;
        }
        public async Task<RegisterResult> RegisterAsync(RegisterUserDto request)
        {
            var result = new RegisterResult();

            var usernameNormalized = request.Username.ToLower();
            var emailNormalized = request.EmailAddress.ToLower();

            if (await _context.Users.AnyAsync(x => x.Username == usernameNormalized))
                result.ValidationErrors.Add(
                    new RegisterUserError { PropertyName = "Username", ErrorMessage = "Username already exists" });

            if (await _context.Users.AnyAsync(x => x.EmailAddress == emailNormalized))
                result.ValidationErrors.Add(new RegisterUserError { PropertyName = "EmailAddress", ErrorMessage = "Email address already exists" });

            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - request.DateOfBirth.Year;

            if (request.DateOfBirth > today.AddYears(-age))
                age--;

            if (age < 18)
            {
                result.ValidationErrors.Add(new RegisterUserError
                {
                    PropertyName = "DateOfBirth",
                    ErrorMessage = "You must be at least 18 years old."
                });
            }
            if (result.ValidationErrors.Count > 0)
            {
                result.Success = false;
                return result;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username.ToLower(),
                DisplayUsername = request.Username,
                EmailAddress = request.EmailAddress.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password),
                DateOfBirth = request.DateOfBirth,
                ProfileImgUrl = string.Empty
            };
            result.UserId = user.Id;
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = 1,
            };
            await _context.UserRoles.AddAsync(userRole);
            await _context.Users.AddAsync(user);
            return result;
        }
    }
}

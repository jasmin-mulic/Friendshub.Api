using BCrypt.Net;
using Friendshub.Application.DTO.Auth;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Application.Results;
using Friendshub.Domain.Models;
namespace Friendshub.Application.Implementations
{
    internal class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService)
        {   
              _unitOfWork = unitOfWork;
              _tokenService = tokenService;
        }
        public async Task<LoginResult> LoginAsync(LoginUserDto request)
        {
            var querryNormalized = request.UsernameOrEmail.ToLower();
            var result = new LoginResult();
            var user = await _unitOfWork.UserRepository.GetUserByEmailOrUsername(querryNormalized);

            if (user == null || BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.PasswordHash) == false)
            {
                result.Success = false;
                return result;
            }
            result.Success = true;
            result.User = user;
            result.AccessToken = await _tokenService.CreateAccessToken(user);
            return result;
        }
        public async Task<RegisterResult> RegisterAsync(RegisterUserDto request)
        {
            var result = new RegisterResult();

            var usernameNormalized = request.Username.ToLower();
            var emailNormalized = request.EmailAddress.ToLower();

            if (await _unitOfWork.UserRepository.IsUsernameTaken(usernameNormalized))
                result.ValidationErrors.Add(
                    new RegisterUserError { PropertyName = "Username", ErrorMessage = "Username already exists" });

            if (await _unitOfWork.UserRepository.IsEmailAddressTaken(emailNormalized))
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
                EmailAddress = request.EmailAddress.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password),
                DateOfBirth = request.DateOfBirth,
                ProfileImageUrl = null
            };
            result.UserId = user.Id;
            result.Success = true;

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = 1,
            };

            await _unitOfWork.UserRoleRepository.AddASync(userRole);
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.ApplyChangesAsync();
            return result;
        }
        public async Task<bool> DeleteAccountAsync(Guid userId, string password)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(userId);
            var isPasswordCorrect = BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash);

            if (!isPasswordCorrect)
                throw new UnauthorizedAccessException
                    ("Wrong password.");

            if (user == null)
                throw new ApplicationException("Your account might be banned. Try logging in again.");
            if (user.ProfileImageUrl != null)
            {
                var directoryPath = Path.Combine("wwwroot", user.ProfileImageUrl);
                if (File.Exists(directoryPath))
                    File.Delete(directoryPath);
            }

            var follows = await _unitOfWork.FollowRepository.GetUserFollowingsList(userId);
            _unitOfWork.FollowRepository.RemoveFollows(follows);
            _unitOfWork.UserRepository.DeleteUser(user);
           await _unitOfWork.ApplyChangesAsync();
            return true;
        }
    }
}

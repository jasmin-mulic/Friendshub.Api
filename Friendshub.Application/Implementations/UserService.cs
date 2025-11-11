using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Extensions;
using Friendshub.Application.Interfaces.Repositories;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using System.Net.Mail;

namespace Friendshub.Application.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<User> GetByIdAsNoTracking(Guid id)
        {
            return await _unitOfWork.UserRepository.GetByIdAsNoTracking(id);
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetUserById(id);
            return user;
        }

        public async Task<LoggedUserData> GetLoggedUserData(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsNoTracking(userId);
            if (user == null)
                return null;

            var followersCount = await _unitOfWork.FollowRepository.GetUserFollowersCount(userId);
            var followingCount = await _unitOfWork.FollowRepository.GetFollowingCount(userId);
            var postCount = await _unitOfWork.PostRepository.GetUserPostCount(userId);

            return new LoggedUserData
            {
                Username = user.Username,
                ProfileImageUrl = user.ProfileImageUrl?.ToFullImageUrl(),
                FollowersCount = followersCount,
                FollowingCount = followingCount,
                EmailAddress = user.EmailAddress,
                PostCount = postCount,
                PrivateAccount = user.PrivateAccount
            };
        }

        public async Task<User> GetUserByEmailOrUsername(string emailOrUsername)
        {
            return await _unitOfWork.UserRepository.GetUserByEmailOrUsername(emailOrUsername);
        }

        public async Task<UserProfileData> GetUserProfileData(string username)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsNoTracking(username);
            if (user == null)
                return null;

            var followersCount = await _unitOfWork.FollowRepository.GetUserFollowersCount(user.Id);
            var followingCount = await _unitOfWork.FollowRepository.GetFollowingCount(user.Id);
            var postCount = await _unitOfWork.PostRepository.GetUserPostCount(user.Id);

            return new UserProfileData
            {
                Username = user.Username,
                ProfileImageUrl = user.ProfileImageUrl?.ToFullImageUrl(),
                FollowersCount = followersCount,
                FollowingCount = followingCount,
                PostCount = postCount,
                PrivateAccount = user.PrivateAccount
               
            };
        }

        public async Task<bool> IsEmailAddressTaken(string emailAddress)
        {
            return await _unitOfWork.UserRepository.IsEmailAddressTaken(emailAddress);
        }

        public async Task<bool> IsUsernameTaken(string username)
        {
            return await _unitOfWork.UserRepository.IsUsernameTaken(username);
        }

        public async Task<Dictionary<string, string>> UpdateUserData(Guid id, UpdateUserInfoDto updateUserInfo)
        {
            var errors = new Dictionary<string, string>();
            var user = await _unitOfWork.UserRepository.GetUserById(id);
            if (user == null)
                throw new NullReferenceException("Your account is either deleted or banned.");
    
            if(user.Username != updateUserInfo.Username.ToLower())
                user.Username = updateUserInfo.Username;
            if (user.EmailAddress != updateUserInfo.EmailAddress.ToLower())
                user.EmailAddress = updateUserInfo.EmailAddress.ToLower();
            if(user.PrivateAccount != updateUserInfo.PrivateAccount)
                user.PrivateAccount = updateUserInfo.PrivateAccount;

            user.Username = updateUserInfo.Username.ToLower();
            user.EmailAddress = updateUserInfo.EmailAddress.ToLower();

            // ✅ upload nove slike ako je poslata
            if (updateUserInfo.ProfileImageUrl != null && updateUserInfo.ProfileImageUrl.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var fileExtension = Path.GetExtension(updateUserInfo.ProfileImageUrl.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    errors.Add("image", "Only JPG, JPEG, PNG or WEBP formats are allowed.");
                    return errors;
                }

                // folder za upload
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "users", "profileImages");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // ✅ obriši staru sliku ako postoji
                if (!string.IsNullOrWhiteSpace(user.ProfileImageUrl))
                {
                    var oldFilePath = Path.Combine("wwwroot", user.ProfileImageUrl.TrimStart('/'));
                    if (File.Exists(oldFilePath))
                        File.Delete(oldFilePath);
                }

                // novi naziv fajla
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(uploadsFolder, fileName);

                // snimi novu sliku
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateUserInfo.ProfileImageUrl.CopyToAsync(stream);
                }

                // snimi URL (relativnu putanju u bazu)
                user.ProfileImageUrl = $"/uploads/users/profileImages/{fileName}";
            }

            _unitOfWork.UserRepository.UpdateUserInfo(user);
            await _unitOfWork.ApplyChangesAsync();

            return errors;
        }

    }
}

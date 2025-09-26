using Friendshub.Api.Extensions;
using Friendshub.Application.DTO.User;
using Friendshub.Application.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Friendshub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetProfileDetails()
        {
            var userIdFromClaims = User.GetUserId();
            if(userIdFromClaims == Guid.Empty)
                return Unauthorized();

            var user = await _unitOfWork.UserRepository.GetById(userIdFromClaims);
            var userData = await _unitOfWork.UserRepository.GetProfileData(user);
            return Ok(userData);
        }
        [HttpPost("change-profile-picture")]
        [Authorize]
        public async Task<IActionResult> ChangeProfileImage(IFormFile formFile)
        {
            try
            {
                var userId = User.GetUserId();
                if (Guid.Empty == userId)
                    return Unauthorized();
                var fileurl = await _unitOfWork.UserRepository.ChangeProfilePicture(formFile);
                var user = await _unitOfWork.UserRepository.GetById(userId);
                user.ProfileImgUrl = fileurl;
                await _unitOfWork.ApplyChanges();
                return Ok("Profile image changed successfully.");
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet("follow-recommendations")]
        public async Task<IActionResult> GetFriendRecommendation()
        {
            try
            {
                var userIdFromClaims = User.GetUserId();

                if (userIdFromClaims.ToString() == string.Empty)
                    return Unauthorized();

                var followRecommendations = await _unitOfWork.UserRepository.GetFollowRecommendationList(userIdFromClaims);
                return Ok(followRecommendations);
            }
            catch (Exception exc)
            {
                return Unauthorized(exc);
            }
        }
        [Authorize]
            [HttpPost("follow-user")]
            public async Task<IActionResult> FollowUser(string foloweeId)
            {
            try
            {
                var userIdFromClaims = User.GetUserId();

                if (Guid.Empty == userIdFromClaims)
                    return Unauthorized();

                var foloweeToGuid = Guid.Parse(foloweeId);
                _unitOfWork.UserRepository.FollowUser(userIdFromClaims, foloweeToGuid);
                await _unitOfWork.ApplyChanges();
                return Ok();
            }
            catch (Exception exc)
            {
                return StatusCode(500, exc.Message);
            }
        }
    }
}

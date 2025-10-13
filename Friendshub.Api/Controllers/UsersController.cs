using Friendshub.Api.Extensions;
using Friendshub.Application.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Friendshub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("me")]
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

       [HttpPost("follow-user")]
       public async Task<IActionResult> FollowUser(string foloweeId)
            {
            try
            {
                var userIdFromClaims = User.GetUserId();

                if (Guid.Empty == userIdFromClaims)
                    return Unauthorized();

                var foloweeToGuid = Guid.Parse(foloweeId);
               var followMessage =  await _unitOfWork.UserRepository.FollowUser(userIdFromClaims, foloweeToGuid);//vraca followed ili unfollowed
                await _unitOfWork.ApplyChanges();
                return Ok(new {message = followMessage});
            }
            catch (Exception exc)
            {
                return StatusCode(500, exc.Message);
            }
        }
       [HttpPost("delete-user")]
        public async Task<IActionResult> DeleteUser()
        {
            try
            {
            var userIdFromClaims = User.GetUserId();
            if(Guid.Empty == userIdFromClaims)
                return Unauthorized("You are logged out.");
            await _unitOfWork.UserRepository.DeleteUser(userIdFromClaims);
            await _unitOfWork.ApplyChanges();
            return Ok(new { message = "You deleted your account. See you soon :D" });
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }

        [HttpGet("followers")]

        public async Task<IActionResult> GetFollowersList()
        {
            try
            {
                var userIdFromClaims = User.GetUserId();
                if (Guid.Empty == userIdFromClaims)
                    return Unauthorized("You are logged out.");
                var followerList = await _unitOfWork.UserRepository.GetFollowers(userIdFromClaims);
                return Ok(new { followers = followerList });
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }

        [HttpPost("remove-follower/{followeerId}")]
        public async Task<IActionResult> RemoveFollower([FromRoute] Guid followeerId)
        {
            try
            {
                var userIdFromClaims = User.GetUserId();
                if( Guid.Empty == userIdFromClaims)
                    return Unauthorized("You are logged out.");

                _unitOfWork.UserRepository.RemoveFollower(userIdFromClaims, followeerId);
               await _unitOfWork.ApplyChanges();
                return Ok(new { message = "Follower removed." });
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
    }
}

using FluentValidation;
using Friendshub.Api.Extensions;
using Friendshub.Application.DTO.UserDto;
using Friendshub.Application.Repositories;
using Friendshub.Infrastructure.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Authorize]

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfileData()
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var user = await _unitOfWork.UserRepository.GetUserById(userId);
            if (user == null)
                return NotFound("User not found.");
            var userData = await _unitOfWork.UserRepository.GetMyProfileData(user);
            return Ok(userData);
        }
        [Authorize]

        [HttpGet("follow-recommendations")]
        public async Task<IActionResult> GetFriendRecommendation()
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var recommendations = await _unitOfWork.UserRepository.GetFollowRecommendationList(userId);
            return Ok(recommendations);
        }
        [Authorize]

        [HttpPost("follow-user")]
        public async Task<IActionResult> FollowUser(Guid foloweeId)
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();
            if (userId == foloweeId)
                return BadRequest("You can't follow yourself.");

            var followeeGuid = (foloweeId);
            var message = await _unitOfWork.UserRepository.FollowUser(userId, followeeGuid);

            await _unitOfWork.ApplyChanges();
            return Ok(new { message });
        }
        [Authorize]

        [HttpPost("remove-follower/{followeeId}")]
        public async Task<IActionResult> RemoveFollower([FromRoute] Guid followeeId)
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized("You are logged out.");

            _unitOfWork.UserRepository.RemoveFollower(userId, followeeId);
            await _unitOfWork.ApplyChanges();

            return Ok(new { message = "Follower removed." });
        }
        [Authorize]

        [HttpGet("followers-list")]
        public async Task<IActionResult> GetFollowersList()
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized("You are logged out.");

            var followers = await _unitOfWork.UserRepository.GetFollowers(userId);
            return Ok(new { followers });
        }
        [Authorize]

        [HttpGet("following-list")]
        public async Task<IActionResult> GetFollowingList()
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized("You are logged out.");

            var followings = await _unitOfWork.UserRepository.GetFollowings(userId);
            return Ok(new { followings });
        }
        [Authorize]

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserInfo([FromRoute] Guid id, [FromForm] UpdateUserInfoDto request, [FromServices] IValidator<UpdateUserInfoDto> validator)
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized("You are logged out.");
            if (userId != id)
                return Unauthorized("You don't have permissions.");

            var errors = validator.Validate(request);
            if(!errors.IsValid)
                return BadRequest(errors);

            var validationErrors = await _unitOfWork.UserRepository.UpdateUserData(id, request);
            if(validationErrors.Count > 0)
                return BadRequest(new {Errors  = validationErrors});
            await _unitOfWork.ApplyChanges();

            return Ok(validationErrors);
        }
        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserProfileData([FromRoute] string username)
        {
            var userInfo = await _unitOfWork.UserRepository.GetUserProfileData(username);
            if (userInfo == null)
                return NotFound("User does not exist or is deleted.");
            return Ok(userInfo);
        }
    }
}

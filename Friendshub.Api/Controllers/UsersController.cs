using FluentValidation;
using Friendshub.Api.Extensions;
using Friendshub.Application.Features.Users;
using Friendshub.Application.Features.Users.DTO;
using Friendshub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Friendshub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFollowService _followService;
        private readonly INotificationService _notificationService;
        public UsersController
        (
            IUserService userService,
            IFollowService followService,
            INotificationService notificationService
        )
        {
            _userService = userService;
            _followService = followService;
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetLoggedUserProfileData()
        {
            var idFromClaims = User.GetUserId();
            if (idFromClaims == Guid.Empty)
                return Unauthorized();

            var userData = await _userService.GetLoggedUserData(idFromClaims);
            return Ok(userData);
        }

        [Authorize]
        [HttpGet("me/follow-recommendations")]
        public async Task<IActionResult> GetFollowRecommendations([FromQuery]int page = 1)
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var recommendations = await _followService.GetFollowRecommendationList(userId, page);
            return Ok(recommendations);
        }

        [Authorize]
        [HttpPost("{followeeId}/follow")]
        public async Task<IActionResult> FollowUser([FromRoute]Guid followeeId)
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();
            if (userId == followeeId)
                return BadRequest("You can't follow yourself.");

            var message = await _followService.AddFollowAsync(userId, followeeId);

            return Ok(new { message });
        }
        
        [Authorize]
        [HttpDelete("{followeeId}/follow")]
        public async Task<IActionResult> UnfollowUser([FromRoute] Guid followeeId)
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized("You are logged out.");

            var isDeleted = await _followService.RemoveFollowAsync(userId, followeeId);
            if (isDeleted)
            return Ok(new { Message = "User unfollowed." });

            return BadRequest(new { Message = "Error unfollowing user" });
        }
        
        [Authorize]
        [HttpGet("me/followers")]
        public async Task<IActionResult> GetFollowersList()
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized("You are logged out.");

            var followers = await _followService.GetFollowers(userId);
            return Ok(new { followers });
        }
        
        [Authorize]
        [HttpGet("me/followings")]
        public async Task<IActionResult> GetFollowingList()
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized("You are logged out.");

            var followings = await _followService.GetUserFollowingsList(userId);
            return Ok(new { followings });
        }
        
        [Authorize]
        [HttpPut("{id}")]

        [Authorize]
        [HttpDelete("{followerId}/follows")]
        public async Task<IActionResult> RemoveFollower(Guid followerId)
        {
            var loggedUserId = User.GetUserId();
            if (loggedUserId == Guid.Empty)
                return Unauthorized("You are logged out.");

            var removingFollowerResponse = await _followService.RemoveFollowAsync(followerId, loggedUserId);
            return Ok(removingFollowerResponse);
        }
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateUserInfo([FromForm] UpdateUserInfoDto request, [FromServices] IValidator<UpdateUserInfoDto> validator)
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized("You are logged out.");

            var errors = validator.Validate(request);
            if (!errors.IsValid)
                return BadRequest(errors);

            var validationErrors = await _userService.UpdateUserData(userId, request);
            if (validationErrors != null && validationErrors.Count > 0)
                return BadRequest(new { Errors = validationErrors });
            return Ok(validationErrors);
        }
        
        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserProfileData([FromRoute] string username)
        {
            var userInfo = await _userService.GetUserProfileData(username);
            if (userInfo == null)
                return NotFound("User does not exist or is deleted.");
            return Ok(userInfo);
        }

        [Authorize]
        [HttpGet("me/notifications")]
        public async Task<IActionResult> GetUserNotifications([FromQuery] int page = 1)
        {
            var userId = User.GetUserId();
            try
            {
            var notificationsPaged = _notificationService.GetNotificationsAsync(userId, page);
            return Ok(notificationsPaged);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }

        }
    }
}

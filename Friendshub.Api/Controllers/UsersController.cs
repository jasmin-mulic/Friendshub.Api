using FluentValidation;
using Friendshub.Api.Extensions;
using Friendshub.Application.DTO.UserDto;
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
        public UsersController(IUserService userService, IFollowService followService)
        {
            _userService = userService;
            _followService = followService;
        }
        [Authorize]

        [HttpGet("me")]
        public async Task<IActionResult> GetLoggedUserProfileData()
        {
            var userIdFromClaims = User.GetUserId();
            if (userIdFromClaims == Guid.Empty)
                return Unauthorized();

            var userData = await _userService.GetLoggedUserData(userIdFromClaims);
            return Ok(userData);
        }
        [Authorize]

        [HttpGet("follow-recommendations/{page}")]
        public async Task<IActionResult> GetFollowRecommendations([FromRoute]int page)
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var recommendations = await  _followService.GetFollowRecommendationList(userId, page);
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

            var message = await _followService.AddFollowAsync(userId, foloweeId);
            
            return Ok(new { message });
        }
        [Authorize]

        [HttpPost("remove-follower/{followeeId}")]
        public async Task<IActionResult> RemoveFollower([FromRoute] Guid followeeId)
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized("You are logged out.");

           await _followService.RemoveFromFollowers(userId, followeeId);
            return Ok(new { message = "Follower removed." });
        }
        [Authorize]

        [HttpGet("followers-list")]
        public async Task<IActionResult> GetFollowersList()
        {
            var userId = User.GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized("You are logged out.");

            var followers = await _followService.GetUserFollowersList(userId);
            return Ok(new { followers });
        }
        [Authorize]

        [HttpGet("following-list")]
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

            var validationErrors = await _userService.UpdateUserData(userId, request);
            if(validationErrors != null && validationErrors.Count > 0)
                return BadRequest(new {Errors  = validationErrors});
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
    }
}

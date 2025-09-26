using Friendshub.Api.Extensions;
using Friendshub.Application.DTO.Auth;
using Friendshub.Application.Repositories;
using Friendshub.Domain.Models;
using Friendshub.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Friendshub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FriendshubDbContext _context;
        public AuthController(IUnitOfWork unitOfWork, FriendshubDbContext context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = new Dictionary<string, List<string>>();
                    foreach (var entry in ModelState)
                    {
                        if (entry.Value.Errors.Count > 0)
                        {
                            errors[entry.Key] = entry.Value.Errors
                                .Select(e => e.ErrorMessage)
                                .ToList();
                        }
                    }
                    return BadRequest(new { Errors = errors });
                }

                var response = await _unitOfWork.AuthRepository.LoginAsync(request);

                if (response.Success == false)
                    return Unauthorized("Wrong credentials");

                var accessToken = response.AccessToken;
                var refreshToken = await _unitOfWork.TokenRepository.GetUserRefreshToken(response.User);

                if (refreshToken == null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
                {
                  refreshToken = await _unitOfWork.TokenRepository.AddRefreshToken(response.User.Id);
                }
                else
                {
                    refreshToken.ExpiresOnUtc = DateTime.UtcNow.AddDays(7);
                    refreshToken.Token = _unitOfWork.TokenRepository.CreateRefreshToken();
                    _context.RefreshTokens.Update(refreshToken);
                }
                var cookieOptions = new CookieOptions { 
                    HttpOnly = true,
                    Secure = true, 
                    SameSite = SameSiteMode.None, 
                    Expires = refreshToken.ExpiresOnUtc 
                };
                Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

                await _unitOfWork.ApplyChanges();
                return Ok(response.AccessToken);
            }


            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (!ModelState.IsValid)
                    {
                        var errors = new Dictionary<string, List<string>>();
                        foreach (var entry in ModelState)
                        {
                            if (entry.Value.Errors.Count > 0)
                            {
                                errors[entry.Key] = entry.Value.Errors.Select(e => e.ErrorMessage).ToList();
                            }
                        }
                        return BadRequest(new { Errors = errors });
                    }
                }
                var result = await _unitOfWork.AuthRepository.RegisterAsync(registerUser);
                if (result.ValidationErrors.Count == 0)
                {
                    var refreshToken = _unitOfWork.TokenRepository.AddRefreshToken(result.UserId);
                    await _unitOfWork.ApplyChanges();
                    return Ok("You registered successfully");
                }
                else
                    return BadRequest(result.ValidationErrors);
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/"
                }); var userIdFromClaims = User.GetUserId();
                _unitOfWork.TokenRepository.DeleteRefreshToken(userIdFromClaims);
                await _unitOfWork.ApplyChanges();

                return Ok(new { message = "Logged out successfully." });
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Logout failed. Please try again");

            }
        }
        [HttpGet("refresh-token")]
        public async Task<IActionResult> GetNewAccessToken()
        {
            try
            {

            var userIdFromClaims = User.GetUserId();

            if (Guid.Empty == userIdFromClaims)
                return Unauthorized();

            if(!Request.Cookies.TryGetValue("refreshToken", out var refreshTokenValue))
                return Unauthorized("No refresh token found in cookies");

            var refreshToken = await _unitOfWork.TokenRepository.GetRefreshTokenByValue(refreshTokenValue);
            if (refreshToken == null)
                return Unauthorized("Invalid refresh token");

            if (refreshToken.ExpiresOnUtc < DateTime.UtcNow)
                return Unauthorized("Refresh token expired.");

            var user = await _unitOfWork.UserRepository.GetById(userIdFromClaims);

            var newAccessToken = await _unitOfWork.TokenRepository.CreateAccessToken(user);
            return Ok(newAccessToken);

            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }
        }
    }
}

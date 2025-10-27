using FluentValidation;
using Friendshub.Api.Extensions;
using Friendshub.Application.DTO.Auth;
using Friendshub.Application.Repositories;
using Friendshub.Application.Results;
using Friendshub.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                var refreshToken = await _unitOfWork.TokenRepository.GetUserRefreshToken(response.User.Id);

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

                await _unitOfWork.ApplyChangesAsync();
                return Ok(response.AccessToken);
            }


            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserDto registerUser, [FromServices]IValidator<RegisterUserDto> validator)
        {
            try
            {
                var errors = validator.Validate(registerUser);
                if(!errors.IsValid)
                {
                    var registerResult = new RegisterResult();
                    registerResult.Success = false;
                    foreach (var  error in errors.Errors)
                    {
                        registerResult.ValidationErrors.Add(new RegisterUserError()
                        {
                            ErrorMessage = error.ErrorMessage,
                            PropertyName = error.PropertyName,
                        });
                    }
                        return BadRequest(registerResult);
                }
                    
                var result = await _unitOfWork.AuthRepository.RegisterAsync(registerUser);
                if (result.Success)
                {
                    var refreshToken = _unitOfWork.TokenRepository.AddRefreshToken(result.UserId);
                    await _unitOfWork.ApplyChangesAsync();
                    return Ok("You registered successfully");
                }
                else
                    return BadRequest( result);
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
                });
                var userIdFromClaims = User.GetUserId();
                _unitOfWork.TokenRepository.DeleteRefreshToken(userIdFromClaims);
                await _unitOfWork.ApplyChangesAsync();

                return Ok(new { message = "Logged out successfully." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Logout failed. Please try again");
            }
        }
        
        [HttpPost("refresh-token")]
        public async Task<IActionResult> GetNewAccessToken()
        {
            try
            {

            if(!Request.Cookies.TryGetValue("refreshToken", out var refreshTokenValue))
                return Unauthorized("No refresh token found in cookies");

            var refreshToken = await _unitOfWork.TokenRepository.GetRefreshTokenByValue(refreshTokenValue);
            if (refreshToken == null)
                return Unauthorized("Invalid refresh token");

            if (refreshToken.ExpiresOnUtc < DateTime.UtcNow)
                return Unauthorized("Refresh token expired.");

            var user = await _unitOfWork.UserRepository.GetUserById(refreshToken.UserId);

            var newAccessToken = await _unitOfWork.TokenRepository.CreateAccessToken(user);
            return Ok(newAccessToken);

            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }
        }

        [HttpDelete("{password}")]
        public async Task<IActionResult> DeleteAccount(string password)
        {
            try
            {
            var useIdFromClaims = User.GetUserId();
            if (Guid.Empty == useIdFromClaims)
                return Unauthorized( new{ Message =  "You are logged out"});

            var isDeletionSuccess = await _unitOfWork.AuthRepository.DeleteAccountAsync(useIdFromClaims, password);

            if (!isDeletionSuccess)
                return BadRequest("Error deleting your account.");

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Account deleted successfully. See you again :)" });

            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }
    }
}

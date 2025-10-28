using FluentValidation;
using Friendshub.Api.Extensions;
using Friendshub.Application.DTO.Auth;
using Friendshub.Application.Interfaces.Services;
using Friendshub.Application.Repositories;
using Friendshub.Application.Results;
using Microsoft.AspNetCore.Mvc;

namespace Friendshub.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        public AuthController(IAuthService authServuce, ITokenService tokenService, IUnitOfWork unitOfWork )
        {
            _authService = authServuce;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login( [FromBody] LoginUserDto request, [FromServices] IValidator<LoginUserDto> validator)
        {
            try
            {

                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                    return BadRequest(new { Errors = validationResult.Errors });

                // 2️⃣ Pokušaj prijave
                var response = await _authService.LoginAsync(request);
                if (!response.Success)
                    return Unauthorized("Wrong credentials");

                // 3️⃣ Uvijek kreiraj novi refresh token (nova sesija)
                var refreshToken = await _tokenService.AddRefreshToken(response.User.Id);

                // 4️⃣ Postavi cookie (HttpOnly + Secure)
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = refreshToken.ExpiresOnUtc
                };
                Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

                // 5️⃣ Sačuvaj promjene
                await _unitOfWork.ApplyChangesAsync();

                // 6️⃣ Vrati Access token klijentu
                return Ok(response.AccessToken);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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

using Friendshub.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Friendshub.Application.Features.Auth.DTO
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "Enter username or email address.")]
        public string UsernameOrEmail { get; set; }
        [Required(ErrorMessage = "Enter your password.")]
        public string Password { get; set; }
    }
}

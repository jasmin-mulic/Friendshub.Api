using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Friendshub.Application.DTO.UserDto
{
    public class UpdateUserInfoDto
    {
        [Required(ErrorMessage ="Please enter username.")]
        public string Username { get; set; }
        public IFormFile ProfileImageUrl { get; set; } = null;

        [EmailAddress(ErrorMessage = "Please enter valid email address.")]
        public string EmailAddress { get; set; }
        public bool PrivateAccount { get; set; }

    }

}

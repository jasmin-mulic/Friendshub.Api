using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.DTO.Auth
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage ="Username is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage ="Please enter a valid email address.")]
        public string EmailAddress { get; set; }
        public DateOnly DateOfBirth { get; set; }
        [MinLength(6,ErrorMessage ="Password must be a minimum of 6 characters.")]
        public string Password { get; set; }
    }
}

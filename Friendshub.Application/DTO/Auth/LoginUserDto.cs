using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.DTO.Auth
{
    public class LoginUserDto
    {
        [Required(ErrorMessage ="Enter username or email address.")]
        public string UsernameOrEmail { get; set; }
        [Required(ErrorMessage ="Enter your password.")]
        public string Password { get; set; }
    }
}

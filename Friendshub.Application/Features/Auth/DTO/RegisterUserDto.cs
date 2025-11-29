using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Application.Features.Auth.DTO
{
    public class RegisterUserDto
    {
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Password { get; set; }
    }
}

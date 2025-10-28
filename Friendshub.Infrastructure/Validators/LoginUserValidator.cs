using FluentValidation;
using Friendshub.Application.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Infrastructure.Validators
{
    public class LoginUserValidator : AbstractValidator<LoginUserDto>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.UsernameOrEmail).NotEmpty().WithMessage("Enter your username.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Enter your password.");

        }
    }
}

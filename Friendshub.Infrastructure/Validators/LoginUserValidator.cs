using FluentValidation;
using Friendshub.Application.DTO.Auth;

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

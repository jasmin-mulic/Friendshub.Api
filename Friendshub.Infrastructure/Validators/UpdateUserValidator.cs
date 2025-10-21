using FluentValidation;
using Friendshub.Application.DTO.UserDto;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Friendshub.Infrastructure.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserInfoDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username can't be empty.")
                .MinimumLength(3)
                .WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(15)
                .WithMessage("Username can't be longer than 15 characters.")
                .Matches(@"^(?!.*\.\.)(?!\.)(?!.*\.$)[a-zA-Z0-9._]{1,15}$")
                .WithMessage("Username can contain letters, numbers, underscore and dot.");


            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage("Email address can't be empty.")
                .EmailAddress()
                .WithMessage("Email address is not valid.");
        }
    }
}

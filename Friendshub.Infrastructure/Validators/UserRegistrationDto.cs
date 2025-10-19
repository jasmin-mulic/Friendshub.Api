using FluentValidation;
using Friendshub.Application.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Friendshub.Infrastructure.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username can't be empty.")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters long")
            .MaximumLength(15)
            .WithMessage("Username can't be longer than 15 characters")
            .Matches(@"^(?!.*\.\.)(?!\.)(?!.*\.$)[a-zA-Z0-9._]{1,20}$")
            .WithMessage("Username can contain letters, numbers, underscore and dot.");

            RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage("Email address can't be empty.")
            .EmailAddress()
            .WithMessage("Username can't be longer than 15 characters");

            RuleFor(x => x.Password)
            .MinimumLength(7)
            .WithMessage("Password must be a minimum of 7 characters.")
            .Matches(@"^(?=.*([A-Z0-9]))(?=.*[#!$]).+$")
            .WithMessage("Password must contain at least one uppercase letter or digit and one special character (#, !, $).");


            RuleFor(x => x.DateOfBirth)
                    .Must(x => BeAtLeast18YearsOld(x))
                    .WithMessage("You must be at least 18 years old.");

        }
        private bool BeAtLeast18YearsOld(DateOnly dateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Today); 
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth > today.AddYears(-age))
                age--;

            return age >= 18;
        }
    }
}
